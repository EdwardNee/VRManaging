using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ping = System.Net.NetworkInformation.Ping;
using UnityEngine.UI;

/// <summary>
/// Сканирование сети.
/// </summary>
public class PingDevices : MonoBehaviour
{
    ClientOculus clientOculus = ClientOculus.Instance;

    internal static string localIp; //локальный адрес.
    private static string s;
    private static string lastNum;  //айди хоста.

    internal static List<string> completed = new List<string>();    //Те, у кого статус - Success.

    /// <summary>
    /// Метод в Старт.
    /// </summary>
    internal static void CallStart()
    {
        completed = new List<string>();
        s = InitLocalIp();
        lastNum = s.Substring(s.LastIndexOf('.') + 1, s.Length - s.LastIndexOf('.') - 1);
        localIp = s.Substring(0, s.LastIndexOf('.') + 1);
    }

    /// <summary>
    /// Начало сканирования.
    /// </summary>
    internal void AsyncPingCall()
    {
        Debug.Log("Show");
        for (int i = 1; i < 255; i++)
        {
            //Чтобы не проверял сам себя.
            //if (i != int.Parse(lastNum))
            //{
            PingLocalAsync(i);
            //}
        }
    }

    /// <summary>
    /// Отправка пинг сообщений в локальной сети.
    /// </summary>
    internal async void PingLocalAsync(int index)
    {
        Ping ping = new Ping();
        await Task.Run(() =>
        {
            //Отключать выводить и выходить из метода, если есть подключение.
            if (ClientOculus.clientSocket.Connected)
            {
                return;
            }

            try
            {
                ClientOculus.clientSocket.Connect(IPAddress.Parse(localIp + index), DataConnection.port);//"192.168.43."
                Debug.Log(ClientOculus.clientSocket.Connected + localIp + index);
            }
            catch (Exception) { }

            var st = ping.Send($"{localIp}{index}"/*, 500, Encoding.Unicode.GetBytes(";")*/)?.Status;
            if (st == IPStatus.Success)
            {
                completed.Add(localIp + index);
                Debug.Log(index);
            }
        });
        Debug.Log("P");
    }
    //Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


    /// <summary>
    /// Нахождение адреса сети.
    /// </summary>
    static string InitLocalIp()
    {
        string Host = Dns.GetHostName();
        string IP = Dns.GetHostAddresses(Host)[0].ToString();
        return IP;
    }
}
