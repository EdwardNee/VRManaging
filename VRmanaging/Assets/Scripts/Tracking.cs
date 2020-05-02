using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using UnityEngine.Video;

public class Tracking : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private static NetServer netServer;
    public VideoPlayer video;
    private Slider trackingSlider;
    private bool slide = false;
    public AudioSource audio;
    public Slider audioVolumeSlider;

    // Start is called before the first frame update
    void Start()
    {
        netServer = NetServer.Instance;
        trackingSlider = GetComponent<Slider>();
    }

    // Update is called once per frame
    /*async*/
    void Update()
    {
        //if (!slide && video.isPlaying)
        //{
        //    trackingSlider.value = (float)video.frame / video.frameCount;

        //}

        if (netServer != null && netServer.connectedClients.Count <= 0)
        {
            return;
        }

        //Закоменчено.
        //bool isMes = false;
        //if (netServer != null && netServer.allowedReceive & netServer.IsPlaying)
        //{
        //    //Тут будем получать инфу в потоке.
        //    //Закоменчено
        //    double valtrack = 0;
        //    await Task.Run(() =>
        //    {
        //        //Получить
        //        byte[] buffer = new byte[1024];
        //        netServer.client.Receive(buffer);
        //        string mes = Encoding.Unicode.GetString(buffer).Trim();
        //        if (mes.Contains("CF") & netServer.IsPlaying && double.Parse(mes.Replace("CF", "")) > 0)
        //        {
        //            valtrack = ulong.Parse(mes.Replace("CF", ""));
        //            Debug.Log("CONTAIN " + mes.Replace("CF", ""));
        //            isMes = true;
        //        }

        //        //dataSplit = data.Select(float.Parse).ToArray();

        //        //Console.WriteLine(Encoding.Unicode.GetString(buffer) + netServer.client.RemoteEndPoint);

        //        //Debug.Log(video.clockTime);
        //        //if (netServer.IsPlaying)
        //        //{


        //        //}
        //    });
        //    if (isMes)
        //    {
        //        isMes = false;
        //        trackingSlider.value = (float)valtrack
        //                               / (float)NetServer.currentFrameCount; /*(float)video.frame / video.frameCount;*/
        //    }
        //}
    }


    public void OnPointerDown(PointerEventData a)
    {
        slide = true;
    }

    public async void OnPointerUp(PointerEventData a)
    {
        //float frame1 = (float)trackingSlider.value * video.frameCount;
        //video.frame = (long)frame1;

        if (netServer.connectedClients.Count <= 0)
        {
            return;
        }
        float frame = trackingSlider.value * (float)NetServer.currentFrameCount;/*video.frameCount;*/

        //тут отпраялется frame video.
        await Task.Run(() =>
        {
            Debug.Log($"SKIPTOTIME {frame}:{trackingSlider.value} * * {NetServer.currentFrameCount}");
            foreach (var cl in netServer.connectedClients)
            {
                netServer.connectedClients[cl.Key].Send(Encoding.Unicode.GetBytes((frame + "L")));
            }
        });
        slide = false;
        //print(video.frame);
    }

    public async void Volume()
    {
        netServer = NetServer.Instance;
        audio.volume = audioVolumeSlider.value;

        if (netServer != null && netServer.connectedClients.Count <= 0)
        {
            return;
        }

        //Тут отправляется изменение звука.
        await Task.Run(() =>
        {
            if (netServer != null && netServer.connectedClients != null)
                foreach (var cl in netServer.connectedClients)
                {
                    netServer.connectedClients[cl.Key].Send(Encoding.Unicode.GetBytes($"{audioVolumeSlider.value}Vol"));
                }
        });
    }
}
