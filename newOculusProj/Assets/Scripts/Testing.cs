using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
//using System.DirectoryServices;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.Video;
using Ping = System.Net.NetworkInformation.Ping;

public class Testing : MonoBehaviour
{
    public static Testing Instance { get; private set; }
    public AudioSource audio;
    public VideoPlayer videoPlayer;
    delegate void ReqDel(string msg);
    private event ReqDel RequestCheck;

    //private const string ip = "127.0.0.1";   //
    //private const int port = 8080;
    internal static Socket clientSocket;

    private void Start()
    {
        roomNum = String.Empty;
        clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        PingDevices.CallStart();
        //PingDevices.RecursiveCall();
        //s = InitLocalIp();
        //lastNum = s.Substring(s.LastIndexOf('.') + 1, s.Length - s.LastIndexOf('.') - 1);
        //localIp = s.Substring(0, s.LastIndexOf('.') + 1);

        ToStart();
    }


    private string _currentVideos;
    void Awake()
    {
        string[] currentVideos = Directory.GetFiles("C:/Users/22508/Videos/");   //Доступные видеофайлы.  /mnt/sdcard/Movies/
        if (currentVideos.Length > 0)
        {
            foreach (var cv in currentVideos)
            {
                _currentVideos += $"{cv};";
            }
        }
        else
        {
            _currentVideos = "НЕТ ДОСТУПНЫХ ВИДЕО";
        }

        Debug.Log(_currentVideos);
        //Подписывание на методы.
        RequestCheck += ChangeVolume;
        RequestCheck += OpenVideoFile;
        RequestCheck += SkipToTime;
        RequestCheck += ConnectionStatus;
        RequestCheck += VideoState;
        //await Task.Run(() =>
        //{
        //initlocap = InitLocalIp();
        //localIp = initlocap.Substring(0, initlocap.LastIndexOf('.') + 1);
        //clientSocket.Connect(IPAddress.Parse($"192.168.43.89"), 8080);
        //});
    }


    string doing;   //Действие - сообщение.
    internal bool allowedSend;

    public async void ToStart()
    {
        await Task.Run(() =>
        {
            while (!clientSocket.Connected)
            {
                if (roomNum.Trim().Length > 0)
                {
                    try
                    {
                        Debug.Log(PingDevices.localIp + roomNum);
                        clientSocket.Connect((PingDevices.localIp + roomNum), 6843);  //PingDevices.localIp + roomNum
                    }
                    catch (Exception e)
                    {
                    }
                }
                //clientSocket.Connect(new IPEndPoint(System.Net.IPAddress.Parse(PingDevices.localIp + roomNum), 6843))};
                //НЕ foreach, т.к он не может пройтись по измененному списку.
                //for (var i = 0; i < PingDevices.completed.Count; i++)
                //{
                //    try
                //    {
                //        Console.WriteLine(PingDevices.completed[i]);
                //        clientSocket.Connect(IPAddress.Parse(PingDevices.completed[i]), 6843);
                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine(e.Message);
                //    }
                //    if (clientSocket.Connected)
                //    {
                //        Console.WriteLine("ВСЕ ВСЕ ВСЕ" + clientSocket.RemoteEndPoint);
                //        break;
                //    }
                //}
            }



            //Тут было подключение пока не подсоединится

            //await Task.Run(() =>
            //{
            //    clientSocket.Connect(IPAddress.Parse($"192.168.0.106"), 8080);  //10.255.197.135
            //});



            //Закоментировано
            Debug.Log("ВСЕ СЛУЧИЛОСЬ");


            if (clientSocket.Connected)
            {
                //setMaterial = true;
                connectedAddr = PingDevices.localIp + roomNum;
                Debug.Log("ВСЕ ВСЕ ВСЕ" + clientSocket.RemoteEndPoint);
                //break;
            }

            //Debug.Log(videoPlayer.frameCount + " FrameCount");
            ////Получить
            //byte[] buffer = new byte[1024];
            //clientSocket.Receive(buffer);
            //Отправить
            clientSocket.Send(Encoding.Unicode.GetBytes(_currentVideos));

            allowedSend = true;
            //Debug.Log(Encoding.Unicode.GetString(buffer));

            while (true)
            {
                doing = GetMessage();
            }
        });




        //Debug.Log("Подключен и вооружен");

        //doing = String.Empty;
        ////await Task.Run(() =>
        ////{
        ////Thread t = new Thread(() =>
        ////{
        //Debug.Log("Дальше прошла собака");




        //Debug.Log("Ожидаю команды включить видео.");




        ////повторное нажатие кнопки вызывает

        //////Отправить
        ////byte[] buffer = new byte[1024];
        ////clientSocket.Send(Encoding.Unicode.GetBytes(Dns.GetHostName()));
        ////Console.WriteLine(Encoding.Unicode.GetString(buffer));
        ////Console.ReadLine();
    }
    //});

    public void OnConfirmClicked()
    {
        roomNum = inputField.text;
    }

    //t.Start();
    private string connectedAddr;

    //private bool setMaterial;
    public InputField inputField;
    private string roomNum;
    private async void Update()
    {
        //Debug.Log(videoPlayer.frameCount);
        //videoPlayer.Play();
        //if (!clientSocket.Connected)
        //{
        //    roomNum = inputField.text;
        //}
        //if (setMaterial)
        //{
        //    setMaterial = false;
        //    RenderSettings.skybox = mat2;
        //}


        if (IsMessageReceived)
        {
            IsMessageReceived = false;
            RequestCheck?.Invoke(doing);
        }

        if (!allowedSend | !videoPlayer.isPlaying)
        {
            return;
        }
        //Debug.Log($"{videoPlayer.frame}/{(long)videoPlayer.frameCount}");
        ulong sendingFrameCount = 0;
        if (ReadyFrame && videoPlayer.isPlaying & videoPlayer.frameCount > 0)
        {
            sendingFrameCount = videoPlayer.frameCount;
            Debug.Log($"Sending: {sendingFrameCount}");
            //В потоке отправляются frame и frameCount.
            await Task.Run(() =>
            {
                clientSocket.SendTo(Encoding.Unicode.GetBytes($"{sendingFrameCount}Fr"),
                    SocketFlags.None, new IPEndPoint(IPAddress.Parse(connectedAddr), 8080)); //10.255.197.135
                Debug.Log($"Send {sendingFrameCount} to {connectedAddr}");
            });

            ReadyFrame = false;
        }

        //Закоменчено
        //if (videoPlayer.isPlaying)
        //{
        //    var sendingFrame = videoPlayer.frame;
        //    await Task.Run(() =>
        //    {
        //        //if ((long)sendingFrameCount - sendingFrame >= 0)
        //        //{
        //            clientSocket.SendTo(Encoding.Unicode.GetBytes($"{sendingFrame}CF"),
        //                    SocketFlags.None, new IPEndPoint(IPAddress.Parse($"10.255.197.135"), 8080));
        //        //}
        //    });
        //}
    }

    private bool IsMessageReceived;
    public string GetMessage()
    {
        //Получить
        byte[] buffer = new byte[1024];
        clientSocket.Receive(buffer);
        string message = Encoding.Unicode.GetString(buffer).Trim();
        //Debug.Log(message);  //
        IsMessageReceived = true;
        return message;
    }

    /// <summary>
    /// Принимает путь к видеофайлу, который нужно открыть.
    /// </summary>
    void OpenVideoFile(string msg)
    {
        Debug.Log("OPver");
        if (!msg.Contains("/"))    //"/mnt/sdcard/Movies/"
        {
            return;
        }
        videoPlayer.url = msg;
        //videoPlayer.Play();
    }


    private bool ReadyFrame;
    /// <summary>
    /// Принимает сообщения тип State.
    /// </summary>
    void VideoState(string msg)
    {
        if (!msg.Contains("State"))
        {
            return;
        }

        if (msg.Contains("On"))
        {
            if (videoPlayer.isPlaying)
            {
                return;
            }
            //videoPlayer.url = @"/C:/Users/22508/Videos/2020-04-02-1733-08.mp4";
            ReadyFrame = true;
            videoPlayer.Play();
            return;
        }
        if (videoPlayer.isPaused)
        {
            return;
        }
        videoPlayer.Pause();
    }

    /// <summary>
    /// Переключает видео на определенное время.
    /// </summary>
    void SkipToTime(string msg)
    {
        if (!msg.Contains("L"))
        {
            return;
        }
        float currFrame;
        Debug.Log($"SkipToTime: {msg.Replace('.', ',')}");
        //try
        //{
        currFrame = float.Parse(msg.Substring(0, msg.IndexOf('L')).Replace('.', ','));
        //}
        //catch (Exception) { return; }

        videoPlayer.frame = (long)currFrame;
        Debug.Log($"HereFrames: {videoPlayer.frame}");
    }

    /// <summary>
    /// Переключает значение звука.
    /// Принимает значение xxVol
    /// </summary>
    void ChangeVolume(string msg)
    {
        if (!msg.Contains("Vol"))
        {
            return;
        }
        Debug.Log("Here" + msg);
        Debug.Log(msg.Substring(0, 4).Replace('.', ','));
        float volume = float.Parse(msg.Substring(0, 4).Replace('.', ','));/* Replace("Vol", "")*/
        Debug.Log(volume);
        audio.volume = volume;
    }

    /// <summary>
    /// Отключает видео, если сервер пожелал
    /// отсоединить клиента. Принимает сообщения типа CnSt:0_id или 1_id.
    /// </summary>
    void ConnectionStatus(string msg)
    {
        if (!msg.Contains("CnSt"))
        {
            return;
        }
        if (msg.Contains("0_id"))
        {
            videoPlayer.Pause();
            return;
        }
        videoPlayer.Play();
        VideoState("On");   //Тогда нужно не забыть отправить путь к файлу, когда происходит подлкючение заново.
    }
}
