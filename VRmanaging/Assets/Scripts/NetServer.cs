using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class NetServer : MonoBehaviour
{
    public static NetServer Instance { get; private set; }
    private AvailableVideosScript availableVideosScript;
    void Awake()
    {
        availableVideosScript = AvailableVideosScript.Instance;
        Instance = this;
        connectedClients = new Dictionary<int, Socket>();
        existed = new List<string>();
    }

    private const int port = 6843;
    internal Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    internal Dictionary<int, Socket> connectedClients = new Dictionary<int, Socket>();
    static int key = 1;

    static string InitLocalIp()
    {
        string Host = Dns.GetHostName();
        string IP = Dns.GetHostAddresses(Host)[1].ToString();
        return IP;
    }
    void Start()
    {
        string s = InitLocalIp();
        Debug.Log($"Room number - {s.Substring(s.LastIndexOf('.') + 1, s.Length - s.LastIndexOf('.') - 1)}");
        videosPath = String.Empty;
        connectedClients = new Dictionary<int, Socket>();
        ToStart();
    }

    internal List<string> existed = new List<string>();  //Подключенные.
    async void Update()
    {
        if (IsVideoDelivered)
        {
            IsVideoDelivered = false;
            videosDelivered?.Invoke();
        }

        //Если уже подключен, то не подключать.
        foreach (var cl in connectedClients)
        {
            if (!existed.Contains(cl.Value.RemoteEndPoint.ToString()))
            {
                existed.Add(cl.Value.RemoteEndPoint.ToString());
                AddElement(cl.Value.RemoteEndPoint.ToString(), cl.Key.ToString());
            }
        }
        byte[] buffer = new byte[1024];
        //Тут принимается куррент фрэймкоунт видео.
        await Task.Run(() =>
        {
            if (ReadyFrame)
            {
                foreach (var cl in connectedClients)
                {
                    connectedClients[cl.Key].Receive(buffer);
                    break;  //Чтобы принять только от одного.
                }

                string hello = Encoding.Unicode.GetString(buffer).Trim();
                ReadyFrame = false;
                if (hello.Contains("Fr"))
                {
                    currentFrameCount = double.Parse(hello.Replace("Fr", ""));
                }
            }
        });
    }

    internal static double currentFrameCount;
    internal bool IsPlaying;
    internal bool ReadyFrame;
  

    internal bool allowedReceive;
    internal Socket client;
    internal static string videosPath;  //Пути к файлам, получены от очков.
    public async void ToStart()
    {
        await Task.Run(() =>
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Debug.Log("Ожидание подключения");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));

            serverSocket.Listen(5);

            Debug.Log("Listen");


            //new Thread(() =>
            //{
            //Debug.Log(t.Seconds);
            while (true)
            {
                client = serverSocket.Accept();
                Debug.Log("Accept");
                //Adding clients with identified key to Dictionary.
                connectedClients.Add(key++, client);
                //Отправить.
                //client.Send(Encoding.Unicode.GetBytes("Вот от сервера привет"));
                //Получить
                byte[] buffer = new byte[1024];
                //if (key <= 1)
                //{
                    client.Receive(buffer);
               
                    videosPath = Encoding.Unicode.GetString(buffer).Trim();
                    Debug.Log(videosPath);
                //}
                //if (availableVideosScript != null)
                //    availableVideosScript.VideosDelivered(); //Запускаем метод, выводящий список видео.
                IsVideoDelivered = true;
                allowedReceive = true;


                Debug.Log("HELLO");


                ////Получить
                //byte[] buffer1 = new byte[1024];
                //client.Receive(buffer1);
                //Console.WriteLine(Encoding.Unicode.GetString(buffer1) + client.RemoteEndPoint);
            }
        });
    }

    private bool IsVideoDelivered;
    internal delegate void VideosDeliveredDel();
    internal VideosDeliveredDel videosDelivered;

    public Transform panel; //Панель, на которую добавляется список устройств.
    public GameObject devicePrefab;   //Стандартный префаб подключенных устройств.
    public Font font;   //Шрифт текста название подключенного устройства. newDeviceText.

    /// <summary>
    /// Метод выводит доступные устройства на экран.
    /// </summary>
    void AddElement(string device, string ID)
    {
        Transform t = Instantiate(devicePrefab.transform, panel);
        t.name = ID;
        //Текст панели.
        GameObject newDeviceText = new GameObject("ID" + ID, typeof(Text));
        newDeviceText.transform.SetParent(t);
        newDeviceText.GetComponent<Text>().fontSize = 132;
        newDeviceText.GetComponent<Text>().text = device;
        newDeviceText.GetComponent<Text>().font = font;
        newDeviceText.GetComponent<RectTransform>().sizeDelta = new Vector2(877.83f, 259f);
        newDeviceText.GetComponent<Text>().color = new Color(0, 0, 0);
        newDeviceText.GetComponent<RectTransform>().localScale = new Vector3(0.37f, 0.37f, 0);
        newDeviceText.GetComponent<RectTransform>().localPosition = new Vector2(-180f, 37f);
        newDeviceText.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
        newDeviceText.GetComponent<Text>().raycastTarget = false;
    }
}
