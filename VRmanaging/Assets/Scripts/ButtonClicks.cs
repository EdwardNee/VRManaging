using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ButtonClicks : MonoBehaviour
{
    private static NetServer netServer;
    private static AvailableVideosScript avs;

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
        //try
        //{
        if (netServer.connectedClients.Count > 0)
        {
            foreach (var client in netServer.connectedClients)
            {
                client.Value.Disconnect(true);
                client.Value.Dispose();
            }

            //
            //Еще было бы неплохо отключать всех disconectedClients
            //

            netServer.connectedClients.Clear();
            netServer.existed.Clear();
            netServer.serverSocket.Shutdown(SocketShutdown.Both);
            netServer.serverSocket.Dispose();
        }
        //}
        //catch (Exception e)
        //{
        //    Debug.Log(e.Message);
        //}
        Debug.Log("QuitGame");
        Application.Quit();
    }

    /// <summary>
    /// Реагирует на нажатие копки videoStart.
    /// </summary>
    public void ButtonClickedStart()
    {
        netServer.IsPlaying = true;
        foreach (var cl in netServer.connectedClients)
        {
            netServer.connectedClients[cl.Key].Send(Encoding.Unicode.GetBytes($"OnState"));    //7
        }
        netServer.ReadyFrame = true;
    }

    public void ButtonClickedStop()
    {
        netServer.IsPlaying = false;
        foreach (var cl in netServer.connectedClients)
        {
            netServer.connectedClients[cl.Key].Send(Encoding.Unicode.GetBytes($"OffState"));    //7
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
