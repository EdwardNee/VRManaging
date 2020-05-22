using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Настройки показа видео.
/// </summary>
public class AvailableVideosScript : MonoBehaviour
{
    public static AvailableVideosScript Instance { get; private set; }  //Дает инициализировать экземпляр.
    private NetServer netServer;    //Инициализирует экземпляр.
    private static string[] videos; //Пути к видео файлам.
    //public GameObject availableVideosPref;
    public Transform parentPanel;   //Родительская панель для видео.
    public Transform availVidPan;   //Для видео.
    public Font font;

    public Text showCurrVid;    //Название текущего видео.
    public Button videoListBttn;    //Выводит список всех доступных видео.

    private GameView gameView;  //Инициализирует экземпляр.

    // Start is called before the first frame update
    void Start()
    {
        gameView = new GameView();
        Instance = this;
        netServer = NetServer.Instance;
        netServer.videosDelivered = new NetServer.VideosDeliveredDel(VideosDelivered);
        //включить потом.
        availVidPan.GetComponent<Animator>().SetTrigger("OnAv");    //Чтобы панель была невидимой.

    }
    /// <summary>
    /// Метод выводит на parentPanel доступные видео.
    /// </summary>
    internal void VideosDelivered()
    {
        Debug.Log($"Я в АВАИЛАБЛЕ {NetServer.videosPath}");
        videos = NetServer.videosPath.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        string[] formats = { "avi", "vmw", "mp4", "mov" };

        for (int i = 0; i < videos.Length; i++)
        {
            bool isFormat = false;
            for (int j = 0; j < formats.Length; j++)
            {
                if (videos[i].ToLower().Contains(formats[j]))
                {
                    isFormat = true;
                    break;
                }
            }
            if (isFormat)
            {
                InitVideos(videos[i]);
            }
        }
        availVidPan.GetComponent<Animator>().SetTrigger("OffAv");    //Чтобы панель была невидимой.
    }

    /// <summary>
    /// Метод инициализирует панель с доступными для проигрывания видео.
    /// </summary>
    void InitVideos(string fileName)
    {
        GameObject newButton = new GameObject(fileName, typeof(Image), typeof(Button), typeof(LayoutElement));
        newButton.transform.SetParent(parentPanel);
        newButton.GetComponent<RectTransform>().sizeDelta = new Vector2(1171.5f, 216f);
        newButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        newButton.GetComponent<RectTransform>().position = new Vector3(635.75f, -118f, 0);
        var button = newButton.GetComponent<Button>();
        button.onClick.AddListener(() => SendVideoName(fileName));

        //Настроить размеры
        GameObject newVideoText = new GameObject(fileName, typeof(Text));
        newVideoText.transform.SetParent(newButton.transform);    //t
        newVideoText.GetComponent<Text>().fontSize = 132;
        Debug.Log(fileName);
        newVideoText.GetComponent<Text>().text = fileName.Substring(fileName.LastIndexOf('/') + 1, fileName.Length - fileName.LastIndexOf('/') - 1);
        newVideoText.GetComponent<Text>().font = font;

        newVideoText.GetComponent<RectTransform>().sizeDelta = new Vector2(1360f, 350.1f);
        newVideoText.GetComponent<Text>().color = new Color(0, 0, 0);
        newVideoText.GetComponent<RectTransform>().localScale = new Vector3(0.23f, 0.23f, 1);
        newVideoText.GetComponent<RectTransform>().localPosition = new Vector2(-202, 0f);
        newVideoText.GetComponent<Text>().fontSize = 300;
        newVideoText.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
        newVideoText.GetComponent<Text>().raycastTarget = false;
    }

    /// <summary>
    /// Срабатывает на нажатие newButton, которое отправляет
    /// путь к видео, которое нужно для показа.
    /// </summary>
    public void SendVideoName(string videoPath)
    {
        foreach (var cl in DataConnection.connectedClients)
        {
            DataConnection.connectedClients[cl.Key].Send(Encoding.Unicode.GetBytes(videoPath));
        }
        //netServer.client.Send(Encoding.Unicode.GetBytes(videoPath));
        videoListBttn.gameObject.SetActive(true);
        //availVidPan.GetComponent<Animator>().SetTrigger("OnAv");
        showCurrVid.text = videoPath.Substring(videoPath.LastIndexOf('/') + 1, videoPath.Length - videoPath.LastIndexOf('/') - 1);
    }
}
