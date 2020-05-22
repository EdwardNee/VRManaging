using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Взаимодействие с таймингом и звуком.
/// </summary>
public class Tracking : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private static NetServer netServer; //Инициализирует экземпляр.
    public VideoPlayer video;   //Вспомогательный плеер.
    private Slider trackingSlider;  //Слайдер тайминга.
    public AudioSource audioSource; //Вспомогательный транслятор звука.
    public Slider audioVolumeSlider;    //Слайдер звука.
    public Text timetext;   //Показывает текущее время в видео.
    private bool slide = false; //Двигается ли ползунок.

    // Start is called before the first frame update
    void Start()
    {
        netServer = NetServer.Instance;
        trackingSlider = GetComponent<Slider>();
    }

    // Update is called once per frame
    async void Update()
    {
        if (netServer.IsPlaying)
        {
            string mes = String.Empty;
            string time = String.Empty;
            await Task.Run(() =>
            {
                byte[] buffer = new byte[1024];
                try
                {
                    netServer.client.Receive(buffer);
                }
                catch (Exception e) { Debug.Log($"@43Tracking{e.Message}"); }
                mes = Encoding.Unicode.GetString(buffer);
                if (mes.Contains("CF") && float.Parse(mes.Substring(0, mes.IndexOf("CF"))) > 0 && NetServer.currentFrameCount > 0)
                {
                    Debug.Log(float.Parse(mes.Substring(0, mes.IndexOf("CF"))));
                    time = mes.Substring(mes.IndexOf("CF") + 2, mes.Length - mes.IndexOf("CF") - 2);
                }
            });
            if (mes.Trim().Length > 0 && mes.Contains("CF"))
            {
                timetext.text = time;
                trackingSlider.value = float.Parse(mes.Substring(0, mes.IndexOf("CF"))) / (float)NetServer.currentFrameCount;
            }
        }

        if (netServer != null && DataConnection.connectedClients.Count <= 0)
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

    /// <summary>
    /// Положение слайдера изменено.
    /// </summary>
    public void OnPointerDown(PointerEventData a)
    {
        slide = true;
    }

    /// <summary>
    /// Фиксирование изменения положения слайдера.
    /// </summary>
    public void OnPointerUp(PointerEventData a)
    {
        //float frame1 = (float)trackingSlider.value * video.frameCount;
        //video.frame = (long)frame1;

        if (DataConnection.connectedClients.Count <= 0)
        {
            return;
        }
        float frame = trackingSlider.value * (float)NetServer.currentFrameCount;/*video.frameCount;*/

        //тут отпраялется frame video.
        Debug.Log($"@SkipToTime {frame}:{trackingSlider.value} * {NetServer.currentFrameCount}");
        foreach (var cl in DataConnection.connectedClients)
        {
            DataConnection.connectedClients[cl.Key].Send(Encoding.Unicode.GetBytes((frame + "L")));
        }
        slide = false;
    }

    /// <summary>
    /// Отправляется изменение звука всем клиентам.
    /// </summary>
    public void Volume()
    {
        audioSource.volume = audioVolumeSlider.value;
        //Тут отправляется изменение звука.
        try
        {
            foreach (var cl in DataConnection.connectedClients)
            {
                DataConnection.connectedClients[cl.Key].Send(Encoding.Unicode.GetBytes($"{audioVolumeSlider.value}Vol"));
            }
        }
        catch (Exception) { }
    }
}
