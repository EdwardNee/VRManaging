using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainTools : MonoBehaviour
{
    public static MainTools Instance { get; private set; }
    private AvailableVideosScript avs;
    public Sprite[] currentWindow;
    public Button[] windowBttns;

    public Transform optionPanel;
    public Transform playerPanel;
    // Start is called before the first frame update
    void Start()
    {
        avs = AvailableVideosScript.Instance;
        windowBttns[0].enabled = false;
        windowBttns[0].GetComponent<Image>().sprite = currentWindow[1];
    }

    public void SwitchToOption()
    {
        optionPanel.GetComponent<Animator>().SetTrigger("OnOp");
        playerPanel.GetComponent<Animator>().SetTrigger("OffPp");
        windowBttns[0].enabled = false;
        windowBttns[0].GetComponent<Image>().sprite = currentWindow[1];
        windowBttns[1].enabled = true;
        windowBttns[1].GetComponent<Image>().sprite = currentWindow[2];
    }

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
