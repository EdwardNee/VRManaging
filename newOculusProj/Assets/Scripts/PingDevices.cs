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

public class PingDevices : MonoBehaviour
{
    internal static void CallStart()
    {
        s = InitLocalIp();
        lastNum = s.Substring(s.LastIndexOf('.') + 1, s.Length - s.LastIndexOf('.') - 1);
        localIp = s.Substring(0, s.LastIndexOf('.') + 1);
    }
    internal static void RecursiveCall()
    {
        for (int i = 2; i < 255; i++)
        {
            //Чтобы не проверял сам себя.
            //if (i != int.Parse(lastNum))
            //{
                PingLocalAsync(i);
            //}
        }
    }
    internal static readonly List<string> completed = new List<string>();

    internal static async void PingLocalAsync(int index)
    {
        Ping ping = new Ping();
        await Task.Run(() =>
        {
            Debug.Log("ping");
            //Отключать выводить и выходить из метода, если есть подключение.
            if (Testing.clientSocket.Connected)
            {
                return;
            }
            var st = ping.Send($"{localIp}{index}"/*, 500, Encoding.Unicode.GetBytes(";")*/)?.Status;
            Console.WriteLine($"{st} {index}");
            //txt.text += $"\n{index}";
            if (st == IPStatus.Success)
            {
                completed.Add(localIp + index);
                Debug.Log(index);
            }
        });
        Debug.Log("P");
    }
    //Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    internal static string localIp;
    private static string s;
    private static string lastNum;

    static string InitLocalIp()
    {
        
        string Host = Dns.GetHostName();
        string IP = Dns.GetHostAddresses(Host)[1].ToString();
        return IP;
    }


}
