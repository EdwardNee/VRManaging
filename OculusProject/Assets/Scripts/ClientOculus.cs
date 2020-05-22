using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Взаимодействие с сервером.
/// </summary>
public class ClientOculus : MonoBehaviour
{
    public static ClientOculus Instance { get; private set; }
    public AudioSource audio;   //Источник звука.
    public VideoPlayer videoPlayer; //Источник видео.

    delegate void ReqDel(string msg);   //Событийный делегат.
    private event ReqDel RequestCheck;  //Оповещает о получении сообщения.

    internal static Socket clientSocket;    //Сокет клиента.

    private string _currentVideos;  //Видеофайлы.
    string doing;   //Действие - сообщение.
    internal bool allowedSend = false;  //Разрешено отправлять начальный фрейм.

    internal static bool ReadyFrame;    //Можно отправить количество фреймов видео.
    internal static bool IsMessageReceived; //Сообщение о действии получено.
    internal static byte[] buffer = new byte[1024]; //Отправляемый буфер сообщений.

    private string connectedAddr;   //Адрес подключенного.

    private int sTime = 0, pTime = 0;   //Периодичность времени отправки фрейма.


    //Start is called before the first frame update
    private void Start()
    {
        foreach (var VARIABLE in DataConnection.networkID)
        {
            Debug.Log(VARIABLE);
        }
        clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        ToStart();
    }

    //Вызывается во время запуска экзмепляра сценария.
    void Awake()
    {
        _currentVideos += $"{Environment.MachineName}#{SystemInfo.batteryLevel}#";

        try
        {
            string[]
                currentVideos =
                    Directory.GetFiles("/mnt/sdcard/Movies/"); //Доступные видеофайлы.   C:/Users/22508/Videos/


            if (currentVideos.Length > 0)
            {
                foreach (var cv in currentVideos)
                {
                    _currentVideos += $"{cv};";
                }
            }
            else
            {
                _currentVideos = "00Nx0"; //Нет доступных видео.
            }
        }
        catch (Exception e) { Debug.Log(e.Message); }
        //Debug.Log(_currentVideos);
        //Подписывание на методы.
        VideoContent vc = new VideoContent(this);
        RequestCheck += vc.ChangeVolume;
        RequestCheck += vc.OpenVideoFile;
        RequestCheck += vc.SkipToTime;
        RequestCheck += vc.ConnectionStatus;
        RequestCheck += vc.VideoState;
    }


    /// <summary>
    /// Метод в Старт.
    /// </summary>
    public async void ToStart()
    {
        //PingDevices.CallStart();
        //new PingDevices().AsyncPingCall();
        await Task.Run(() =>
        {
            //НУЖНО
            while (!clientSocket.Connected)
            {

                try
                {
                    clientSocket.Connect(IPAddress.Parse(DataConnection.multicastAddr), DataConnection.port);
                    connectedAddr = DataConnection.multicastAddr;
                }
                catch (Exception) { }
                //if (roomNum.Trim().Length > 0)
                //{
                //for (int i = 0; i < DataConnection.networkID.Count; i++)
                //{
                //    try
                //    {
                //        //Debug.Log(DataConnection.networkID[i] + roomNum);
                //        clientSocket.Connect((PingDevices.localIp), DataConnection.port);  //PingDevices.networkID + roomNum
                //    }
                //    catch (Exception) { }

                //    if (clientSocket.Connected)
                //    {
                //        connectedAddr = DataConnection.networkID[i];
                //        Debug.Log("@Connected to" + clientSocket.RemoteEndPoint);
                //        break;
                //    }
                //}
            }


            if (clientSocket.Connected)
            {
                Debug.Log($"Connected to {connectedAddr}");
                //Отправить
                clientSocket.Send(Encoding.Unicode.GetBytes(_currentVideos));
                allowedSend = true;

                //Далее, принимаем сообщения
                while (true)
                {
                    doing = VideoContent.GetMessage();
                }
            }

            //НУЖНО Отправить
            clientSocket.Send(Encoding.Unicode.GetBytes(_currentVideos));
            allowedSend = true;

            //Далее, принимаем сообщения
            while (true)
            {
                doing = VideoContent.GetMessage();
            }
        });
    }


    //Update is called once per frame
    private async void Update()
    {
        sTime = (int)videoPlayer.time;
        string sendTime = $"{(int)(videoPlayer.time / 60)}:{(int)videoPlayer.time % 60}";
        if (IsMessageReceived)
        {
            IsMessageReceived = false;
            RequestCheck?.Invoke(doing);
        }

        if (!allowedSend | !videoPlayer.isPlaying)
        {
            return;
        }
        ulong sendingFrameCount = 0;
        if (ReadyFrame && videoPlayer.isPlaying & videoPlayer.frameCount > 0)
        {
            sendingFrameCount = videoPlayer.frameCount;
            Debug.Log($"Sending: {sendingFrameCount}");
            //В потоке отправляются frame и frameCount.
            await Task.Run(() =>
            {

                clientSocket.SendTo(Encoding.Unicode.GetBytes($"{sendingFrameCount}Fr"),
                    SocketFlags.None, new IPEndPoint(IPAddress.Parse(connectedAddr), DataConnection.port)); //10.255.197.135
                Debug.Log($"Send {sendingFrameCount} to {connectedAddr}");
            });

            ReadyFrame = false;
        }

        //Фреймы
        if (!videoPlayer.isPaused & videoPlayer.frame > 0 & sTime - pTime >= 1)
        {
            var sendingFrame = videoPlayer.frame;
            clientSocket.SendTo(Encoding.Unicode.GetBytes($"{sendingFrame}CF{sendTime}"),
                SocketFlags.None, new IPEndPoint(IPAddress.Parse(connectedAddr), DataConnection.port));
        }
        pTime = sTime;
    }
}

/// <summary>
/// Класс с данными для подключения.
/// </summary>
static class DataConnection
{
    internal static readonly int port = 6843;  //Порт.
    internal static readonly List<string> networkID = InitNetworkID();    //Адреса.
    internal static readonly string multicastAddr = "192.168.0.102";

    /// <summary>
    /// Инициализация NetWorkID.
    /// </summary>
    static List<string> InitNetworkID()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        List<string> tmp = new List<string>();
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                tmp.Add(ip.ToString().Substring(0, ip.ToString().LastIndexOf('.') + 1));
            }
        }
        return tmp;
    }
}

/// <summary>
/// Класс с изменением видео.
/// </summary>
class VideoContent
{
    private ClientOculus clientOculus;

    internal VideoContent(ClientOculus clientOculus)
    {
        this.clientOculus = clientOculus;
    }

    /// <summary>
    /// Принимает сообщения.
    /// </summary>
    internal static string GetMessage()
    {
        //Получить
        ClientOculus.buffer = new byte[1024];
        ClientOculus.clientSocket.Receive(ClientOculus.buffer);
        string message = Encoding.Unicode.GetString(ClientOculus.buffer).Trim();
        ClientOculus.IsMessageReceived = true;
        return message;
    }

    /// <summary>
    /// Принимает путь к видеофайлу, который нужно открыть.
    /// </summary>
    internal void OpenVideoFile(string msg)
    {
        Debug.Log("OPver");
        if (!msg.Contains("/"))    //"/mnt/sdcard/Movies/"
        {
            return;
        }
        clientOculus.videoPlayer.url = msg;
    }

    /// <summary>
    /// Принимает сообщения тип State.
    /// </summary>
    internal void VideoState(string msg)
    {
        if (!msg.Contains("State"))
        {
            return;
        }

        if (msg.Contains("On"))
        {
            if (clientOculus.videoPlayer.isPlaying)
            {
                return;
            }

            ClientOculus.ReadyFrame = true;
            clientOculus.videoPlayer.Play();
            return;
        }
        if (clientOculus.videoPlayer.isPaused)
        {
            return;
        }
        clientOculus.videoPlayer.Pause();
    }


    /// <summary>
    /// Конвертирует данные с плавающей точкой
    /// из-за возможных проблем с кодировкой.
    /// </summary>
    static double ConvertDoubles(string msg)
    {
        //Debug.Log($"MSG {msg.Replace(",", ".")}");
        return double.Parse(msg/*.Replace(".", ",")*/);
    }
    /// <summary>
    /// Переключает видео на определенное время.
    /// </summary>
    internal void SkipToTime(string msg)
    {
        if (!msg.Contains("L"))
        {
            return;
        }
        Debug.Log(msg);
        var data = ConvertDoubles(msg.Substring(0, msg.IndexOf("L")));
        Debug.Log($"SkipToTime: {data}");   //msg.Replace('.', ',')
        var currFrame = (float)data;
        clientOculus.videoPlayer.frame = (long)currFrame;
        Debug.Log($"HereFrames: {clientOculus.videoPlayer.frame}");
    }

    /// <summary>
    /// Переключает значение звука.
    /// Принимает значение xxVol
    /// </summary>
    internal void ChangeVolume(string msg)
    {
        Debug.Log("HEYVOLUME" + msg);
        if (!msg.Contains("Vol"))
        {
            return;
        }
        var values = ConvertDoubles(msg.Substring(0, msg.IndexOf("Vol")));
        Debug.Log($"@ChangeVolume: {values}");    //msg.Substring(0, 4).Replace('.', ',')
        Debug.Log(msg.Substring(0, msg.IndexOf("Vol")));
        clientOculus.audio.volume = (float)values;   /* Replace("Vol", "")*/
        //Debug.Log("Test" + clientOculus.audio.volume);
    }

    /// <summary>
    /// Отключает видео, если сервер пожелал
    /// отсоединить клиента. Принимает сообщения типа CnSt:0_id или 1_id.
    /// </summary>
    internal void ConnectionStatus(string msg)
    {
        if (!msg.Contains("CnSt"))
        {
            return;
        }
        if (msg.Contains("0_id"))
        {
            clientOculus.videoPlayer.Pause();
            return;
        }
        clientOculus.videoPlayer.Play();
        VideoState("On");   //Тогда нужно не забыть отправить путь к файлу, когда происходит подлкючение заново.
    }
}
