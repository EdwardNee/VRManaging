using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionState : MonoBehaviour
{
    public Toggle toggle;
    private static NetServer netServer;
    Dictionary<int, Socket> disconnectedClients = new Dictionary<int, Socket>();
    void Awake()
    {
        //Initializing AddElem instance.
        netServer = NetServer.Instance;

        //Подписываю на событие метод, для реагирования.
        toggle.onValueChanged.AddListener(delegate { OnToggleChanged(toggle); });
    }
    

    /// <summary>
    /// Метод реагирует на изменение значения toggle.
    /// </summary>
    void OnToggleChanged(Toggle tog)
    {
        if (tog.isOn)
        {
            GetComponent<Text>().text = "Connected";
            if (disconnectedClients.Count != 0)
            {
                netServer.connectedClients.Add(int.Parse(toggle.transform.parent.name),
                    disconnectedClients[int.Parse(toggle.transform.parent.name)]);
                disconnectedClients.Remove(int.Parse(toggle.transform.parent.name)); //Удаляем из списка отключенных.
                Debug.Log(int.Parse(toggle.transform.parent.name) + " Connected");
            }
            return;
        }


        GetComponent<Text>().text = "Disonnected";
        if (netServer != null)
        {
            Debug.Log($"parent: {toggle.transform.parent}");
            disconnectedClients.Add(int.Parse(toggle.transform.parent.name),    //Добавляется в словарь отключенных.
                netServer.connectedClients[int.Parse(toggle.transform.parent.name)]);

            netServer.connectedClients.Remove(int.Parse(toggle.transform.parent.name)); //Удаляем из списка подключенных, ничего не транслируется.
            Debug.Log(int.Parse(toggle.transform.parent.name) + " Disconnected");
            //Тут должно быть отправка сообщения о завершении видео.
            foreach (var dcl in disconnectedClients)
            {
                disconnectedClients[dcl.Key].Send(Encoding.Unicode.GetBytes("OfState"));
            }

            //GameObject.Find("нужный объект с компонентом").transform;
        }
    }
    void Update()
    {
        //Debug.Log($"{disconnectedClients.Count}d : {netServer.connectedClients.Count}c");
        //(AddElem.Instance.smth);
        //GameObject toggleObj = GameObject.Find("1/Toggle");   //Найти Toggle. У 1 может быть родитель.
        //Debug.Log(AddElem.FindObjectsOfType(typeof(Dictionary<int, Socket>)));
    }
}
