using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Основное подключение.
/// </summary>
public class NetServer : MonoBehaviour
{
    public static NetServer Instance { get; private set; }
    private static GameView gameView;   //Инициализирует экземпляр.
    private AvailableVideosScript availableVideosScript;    //Инициализирует экземпляр.

    internal Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //Сокет сервера.

    internal Socket client; //Сокет подключенного клиента.
    internal static string videosPath;  //Пути к файлам, получены от очков.
    internal List<string> existed = new List<string>();  //Подключенные.

    //Видео доставлено.
    private bool IsVideoDelivered;  //Доставлено видео или нет.
    internal delegate void VideosDeliveredDel();    //Видео Доставлено
    internal VideosDeliveredDel videosDelivered;    //Экземпляр делегата.


    //Работа с видео.
    internal static double currentFrameCount; //Количество фреймов данного видео.
    internal bool IsPlaying;    //Воспроизводится видео или нет.
    internal bool ReadyFrame;   //Можно ли принять количество фреймов видео, нужно, чтобы не было конфликтов сообщений.
    internal bool allowedReceive;   //Можно принимать фреймы.

    public Transform panel; //Панель, на которую добавляется список устройств.
    public GameObject devicePrefab;   //Стандартный префаб подключенных устройств.
    public Font font;   //Шрифт текста название подключенного устройства. newDeviceText.

    public Text roomNumber; //Номер комнаты.

    //Вызывается во время запуска экзмепляра сценария.
    void Awake()
    {
        availableVideosScript = AvailableVideosScript.Instance;
        Instance = this;
        existed = new List<string>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameView = new GameView();
        string localIp = DataConnection.hostID;
        roomNumber.text += $"\n<b>{localIp.Substring(localIp.LastIndexOf('.') + 1, localIp.Length - localIp.LastIndexOf('.') - 1)}</b>";
        Debug.Log($"@Room number - {roomNumber}");
        videosPath = String.Empty;
        DataConnection.connectedClients = new Dictionary<int, Socket>();
        ToStart();
    }

    // Update is called once per frame
    async void Update()
    {
        if (IsVideoDelivered)
        {
            IsVideoDelivered = false;
            videosDelivered?.Invoke();
        }

        //Если уже подключен, то не подключать.
        int i = 0;
        foreach (var cl in DataConnection.connectedClients)
        {
            if (!existed.Contains(cl.Value.RemoteEndPoint.ToString()) && DataConnection.deviceState.Count > 0)
            {
                existed.Add(cl.Value.RemoteEndPoint.ToString());
                gameView.AddElement(DataConnection.deviceState.Keys.ToList()[i],
                    DataConnection.connectedClients.Keys.ToList()[i].ToString(), DataConnection.deviceState.Values.ToList()[i]);
                //AddElement(cl.Value.RemoteEndPoint.ToString(), cl.Key.ToString(), );
                i++;
            }
        }

        byte[] buffer = new byte[1024];
        //Тут принимается куррент фрэймкоунт видео.
        await Task.Run(() =>
        {
            if (ReadyFrame)
            {
                Debug.Log("ReadyFrame");
                foreach (var cl in DataConnection.connectedClients)
                {
                    try
                    {
                        DataConnection.connectedClients[cl.Key].Receive(buffer);
                    }
                    catch (Exception) { }
                    break;  //Чтобы принять только от одного.
                }

                string hello = Encoding.Unicode.GetString(buffer).Trim();
                ReadyFrame = false;
                Debug.Log($"hELLO {hello}");
                if (hello.Contains("Fr"))
                {
                    currentFrameCount = double.Parse(hello.Replace("Fr", ""));
                }
            }
        });
    }
    /// <summary>
    /// Метод в старт.
    /// </summary>
    public async void ToStart()
    {
        await Task.Run(() =>
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Debug.Log("@NetServer: Ожидание подключения");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, DataConnection.port));
            serverSocket.Listen(5);
            Debug.Log("@NetServer: Listen");

            while (true)
            {
                client = serverSocket.Accept();
                Debug.Log("@NetServer: Accepting");
                //Adding clients with identified key to Dictionary.
                DataConnection.connectedClients.Add(DataConnection.key++, client);
                //Получить
                byte[] buffer = new byte[1024];
                client.Receive(buffer);
                var vam = Encoding.Unicode.GetString(buffer).Trim();//Название. Заряд. Видеофайлы.

                try
                {
                    DataConnection.deviceState.Add(vam.Substring(0, vam.IndexOf("#")),
                        float.Parse($"{vam.Substring(vam.IndexOf("#") + 1, vam.LastIndexOf("#") - vam.IndexOf("#") - 1)}"));
                }
                catch (Exception) { Debug.Log("@Ошибка - такой уже был добавлен."); }
                //Чтобы только один мог отправить видео.
                if (!videosPath.Contains(vam.Substring(vam.LastIndexOf("#") + 1, vam.Length - vam.LastIndexOf("#") - 1)))
                {
                    videosPath = vam.Substring(vam.LastIndexOf("#") + 1, vam.Length - vam.LastIndexOf("#") - 1);
                }
                IsVideoDelivered = true;
                allowedReceive = true;
                Debug.Log($"@NetServer: HELLO, client {DataConnection.key}");
            }
        });
    }
}

/// <summary>
/// Класс-интерфейс.
/// </summary>
public class GameView : MonoBehaviour
{
    NetServer netServer = NetServer.Instance;
    //AvailableVideosScript avs = AvailableVideosScript.Instance;
    /// <summary>
    /// Метод выводит доступные устройства на экран.
    /// </summary>
    internal void AddElement(string device, string ID, float batteryLevel)
    {
        netServer = NetServer.Instance;

        Transform t = Instantiate(netServer.devicePrefab.transform, netServer.panel);
        t.name = ID;
        //Текст панели.
        GameObject newDeviceText = new GameObject("ID" + ID, typeof(Text));
        newDeviceText.transform.SetParent(t);
        newDeviceText.GetComponent<Text>().fontSize = 132;
        newDeviceText.GetComponent<Text>().text = device;
        newDeviceText.GetComponent<Text>().font = netServer.font;
        newDeviceText.GetComponent<RectTransform>().sizeDelta = new Vector2(877.83f, 259f);
        newDeviceText.GetComponent<Text>().color = new Color(0, 0, 0);
        newDeviceText.GetComponent<RectTransform>().localScale = new Vector3(0.4f, 0.4f, 0);
        newDeviceText.GetComponent<RectTransform>().localPosition = new Vector2(-180f, -37.5f);
        newDeviceText.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
        newDeviceText.GetComponent<Text>().raycastTarget = false;

        //Уровень заряда
        GameObject newDeviceBatt = new GameObject("IDs" + batteryLevel, typeof(Text));
        newDeviceBatt.transform.SetParent(t);
        newDeviceBatt.GetComponent<Text>().fontSize = 132;
        newDeviceBatt.GetComponent<Text>().text = $"{batteryLevel * 100}%";
        newDeviceBatt.GetComponent<Text>().font = netServer.font;
        newDeviceBatt.GetComponent<RectTransform>().sizeDelta = new Vector2(877.83f, 259f);
        newDeviceBatt.GetComponent<Text>().color = new Color(0, 0, 0);
        newDeviceBatt.GetComponent<RectTransform>().localScale = new Vector3(0.4f, 0.4f, 0);
        newDeviceBatt.GetComponent<RectTransform>().localPosition = new Vector2(391.8f, -37.5f);
        newDeviceBatt.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
        newDeviceBatt.GetComponent<Text>().raycastTarget = false;
    }
}