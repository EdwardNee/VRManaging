using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

/// <summary>
/// Класс содержит методы для кнопок.
/// </summary>
public class ButtonClicks : MonoBehaviour
{
    private static NetServer netServer;
    private static AvailableVideosScript avs;

    // Start is called before the first frame update 
    void Start()
    {
        netServer = NetServer.Instance;
        avs = AvailableVideosScript.Instance;
    }
    /// <summary>
    /// Реагирует на нажатие кнопки exit.
    /// </summary>
    public void ExitGame()
    {
        //На случай, когда выйти хочется, так и не подключившись.
        try
        {
            if (DataConnection.connectedClients.Count > 0)
            {
                foreach (var client in DataConnection.connectedClients)
                {
                    DataConnection.connectedClients[client.Key].Send(Encoding.Unicode.GetBytes($"OffState"));
                    client.Value.Disconnect(true);
                    client.Value.Dispose();
                }
                foreach (var client in DataConnection.disconnectedClients)
                {
                    client.Value.Disconnect(true);
                    client.Value.Dispose();
                }

                DataConnection.connectedClients.Clear();
                DataConnection.disconnectedClients.Clear();
                
                netServer.existed.Clear();
                netServer.serverSocket.Shutdown(SocketShutdown.Both);
                netServer.serverSocket.Dispose();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        Debug.Log("QuitGame");
        Application.Quit();
    }
    //Отправляется всем игровым объектам перед выходом из приложения.
    void OnApplicationQuit()
    {
        ExitGame();
    }

    /// <summary>
    /// Реагирует на нажатие копки videoStart.
    /// </summary>
    public void ButtonClickedStart()
    {
        netServer.IsPlaying = true;
        foreach (var cl in DataConnection.connectedClients)
        {
            DataConnection.connectedClients[cl.Key].Send(Encoding.Unicode.GetBytes($"OnState"));    //7
        }
        netServer.ReadyFrame = true;
    }

    /// <summary>
    /// Реагируетс на нажатие кнопки videoStop.
    /// </summary>
    public void ButtonClickedStop()
    {
        netServer.IsPlaying = false;
        foreach (var cl in DataConnection.connectedClients)
        {
            DataConnection.connectedClients[cl.Key].Send(Encoding.Unicode.GetBytes($"OffState"));    //7
        }
        //netServer.ReadyFrame = true;
    }
    public void ButtonVideoList()
    {
        avs = AvailableVideosScript.Instance;
        avs.availVidPan.GetComponent<Animator>().SetTrigger("OffAv");
        //avs.videoListBttn.gameObject.SetActive(false);
    }
}
