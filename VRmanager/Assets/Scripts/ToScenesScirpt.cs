using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ToScenesScirpt : MonoBehaviour
{
    public GameObject playerBttn;
    public void SceneLoad(int index) => SceneManager.LoadScene(index);
}
