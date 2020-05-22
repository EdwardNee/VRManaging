using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Переключение между панелями: плеер и настройки.
/// </summary>
public class MainTools : MonoBehaviour
{
    private AvailableVideosScript avs { get; set; }
    public Sprite[] currentWindow;  //Спрайты кнопок.
    public Button[] windowBttns;    //Кнопки переключения панелей.

    public Transform optionPanel;   //Панель настроек.
    public Transform playerPanel;   //Панель с плеером.
    // Start is called before the first frame update
    void Start()
    {
        avs = AvailableVideosScript.Instance;
        windowBttns[0].enabled = false;
        windowBttns[0].GetComponent<Image>().sprite = currentWindow[1];
    }

    /// <summary>
    /// Реагирует при нажатии на кнопку OptionBttn.
    /// </summary>
    public void SwitchToOption()
    {
        optionPanel.GetComponent<Animator>().SetTrigger("OnOp");
        playerPanel.GetComponent<Animator>().SetTrigger("OffPp");
        windowBttns[0].enabled = false;
        windowBttns[0].GetComponent<Image>().sprite = currentWindow[1];
        windowBttns[1].enabled = true;
        windowBttns[1].GetComponent<Image>().sprite = currentWindow[2];
    }

    /// <summary>
    /// Реагирует при нажатии на кнопку PlayerBttn.
    /// </summary>
    public void SwitchToPlayer()
    {
        playerPanel.GetComponent<Animator>().SetTrigger("OnPp");
        optionPanel.GetComponent<Animator>().SetTrigger("OffOp");
        windowBttns[1].enabled = false;
        windowBttns[1].GetComponent<Image>().sprite = currentWindow[3];
        windowBttns[0].enabled = true;
        windowBttns[0].GetComponent<Image>().sprite = currentWindow[0];
    }
}
