//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityEngine.UI;

//public class HelloSceneScript : MonoBehaviour
//{
//    internal static Socket clientSocket;
//    public InputField inputField;
//    private string roomNum;
//    private void Start()
//    {
//        roomNum = String.Empty;
//        clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
//        PingDevices.CallStart();
//        //PingDevices.RecursiveCall();
//        //s = InitLocalIp();
//        //lastNum = s.Substring(s.LastIndexOf('.') + 1, s.Length - s.LastIndexOf('.') - 1);
//        //localIp = s.Substring(0, s.LastIndexOf('.') + 1);

//        ToStart();
//    }

//    public async void ToStart()
//    {
//        await Task.Run(() =>
//        {
//            while (!clientSocket.Connected)
//            {
//                if (roomNum.Trim().Length > 0)
//                {
//                    try
//                    {
//                        Debug.Log(PingDevices.localIp + roomNum);
//                        clientSocket.Connect(IPAddress.Parse(PingDevices.localIp + roomNum), 6843);
//                    }
//                    catch (Exception e) { }
//                }
//                //clientSocket.Connect(new IPEndPoint(System.Net.IPAddress.Parse(PingDevices.localIp + roomNum), 6843))};
//                //НЕ foreach, т.к он не может пройтись по измененному списку.
//                //for (var i = 0; i < PingDevices.completed.Count; i++)
//                //{
//                //    try
//                //    {
//                //        Console.WriteLine(PingDevices.completed[i]);
//                //        clientSocket.Connect(IPAddress.Parse(PingDevices.completed[i]), 6843);
//                //    }
//                //    catch (Exception e)
//                //    {
//                //        Console.WriteLine(e.Message);
//                //    }
//                //    if (clientSocket.Connected)
//                //    {
//                //        Console.WriteLine("ВСЕ ВСЕ ВСЕ" + clientSocket.RemoteEndPoint);
//                //        break;
//                //    }
//                //}
//            }



//            //Тут было подключение пока не подсоединится

//            //await Task.Run(() =>
//            //{
//            //    clientSocket.Connect(IPAddress.Parse($"192.168.0.106"), 8080);  //10.255.197.135
//            //});



//            //Закоментировано
//            Debug.Log("ВСЕ СЛУЧИЛОСЬ");


//            if (clientSocket.Connected)
//            {
//                Debug.Log("ВСЕ ВСЕ ВСЕ" + clientSocket.RemoteEndPoint);
//                //break;
//            }

//            //Debug.Log(videoPlayer.frameCount + " FrameCount");
//            ////Получить
//            //byte[] buffer = new byte[1024];
//            //clientSocket.Receive(buffer);
//            //Отправить
//            clientSocket.Send(Encoding.Unicode.GetBytes(_currentVideos));

//            allowedSend = true;
//            //Debug.Log(Encoding.Unicode.GetString(buffer));

//            while (true)
//            {
//                doing = Testing.GetMessage();
//            }
//        });






//    //Debug.Log("Подключен и вооружен");

//    //doing = String.Empty;
//    ////await Task.Run(() =>
//    ////{
//    ////Thread t = new Thread(() =>
//    ////{
//    //Debug.Log("Дальше прошла собака");




//    //Debug.Log("Ожидаю команды включить видео.");




//    ////повторное нажатие кнопки вызывает

//    //////Отправить
//    ////byte[] buffer = new byte[1024];
//    ////clientSocket.Send(Encoding.Unicode.GetBytes(Dns.GetHostName()));
//    ////Console.WriteLine(Encoding.Unicode.GetString(buffer));
//    ////Console.ReadLine();
//}

//    void Update()
//    {
//        if (!clientSocket.Connected)
//        {
//            roomNum = inputField.text;
//        }
//    }
//}
