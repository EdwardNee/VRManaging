using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Информация о подключенных клиентах.
/// </summary>
public class ConnectionState : MonoBehaviour
{
    public Toggle toggle;   //Переключатель.
    private static NetServer netServer; //Инициализирует экземпляр.

    //Вызывается во время запуска экзмепляра сценария.
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
            if (DataConnection.disconnectedClients.Count != 0)
            {
                DataConnection.connectedClients.Add(int.Parse(toggle.transform.parent.name),
                    DataConnection.disconnectedClients[int.Parse(toggle.transform.parent.name)]);
                DataConnection.disconnectedClients.Remove(int.Parse(toggle.transform.parent.name)); //Удаляем из списка отключенных.
                Debug.Log(int.Parse(toggle.transform.parent.name) + " Connected");
            }
            return;
        }

        GetComponent<Text>().text = "Disonnected";
        if (netServer != null)
        {
            Debug.Log($"parent: {toggle.transform.parent}");
            DataConnection.disconnectedClients.Add(int.Parse(toggle.transform.parent.name),    //Добавляется в словарь отключенных.
                DataConnection.connectedClients[int.Parse(toggle.transform.parent.name)]);

            DataConnection.connectedClients.Remove(int.Parse(toggle.transform.parent.name)); //Удаляем из списка подключенных, ничего не транслируется.
            Debug.Log(int.Parse(toggle.transform.parent.name) + " Disconnected");
            //Тут должно быть отправка сообщения о завершении видео.
            foreach (var dcl in DataConnection.disconnectedClients)
            {
                DataConnection.disconnectedClients[dcl.Key].Send(Encoding.Unicode.GetBytes("OfState"));
            }
            //GameObject.Find("нужный объект с компонентом").transform;
        }
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"{disconnectedClients.Count}d : {netServer.connectedClients.Count}c");
        //(AddElem.Instance.smth);
        //GameObject toggleObj = GameObject.Find("1/Toggle");   //Найти Toggle. У 1 может быть родитель.
        //Debug.Log(AddElem.FindObjectsOfType(typeof(Dictionary<int, Socket>)));
    }
}
