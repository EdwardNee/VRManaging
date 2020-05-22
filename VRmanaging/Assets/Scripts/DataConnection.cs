using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// Класс с данными для подключения.
/// </summary>
static class DataConnection
{
    internal static Dictionary<int, Socket> connectedClients = new Dictionary<int, Socket>();  //Словарь подключений.
    internal static Dictionary<int, Socket> disconnectedClients = new Dictionary<int, Socket>();    //C кем прекращено взаимодействие.
    internal static Dictionary<string, float> deviceState = new Dictionary<string, float>();    //Состояние подлюченных.
    internal static int key = 1; //Ключ подлючения.
    internal static readonly int port = 6843;  //Порт.
    internal static readonly string hostID = InitHostID();    //HostID.
    /// <summary>
    /// Инициализирует hostID.
    /// </summary>
    /// <returns></returns>
    static string InitHostID()
    {
        string Host = Dns.GetHostName();
        string IP = Dns.GetHostAddresses(Host)[0].ToString();
        return IP;
    }
}
