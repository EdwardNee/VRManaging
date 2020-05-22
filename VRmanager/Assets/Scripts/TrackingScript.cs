using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;

public class TrackingScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public VideoPlayer video;
    private Slider tracking;
    private bool slide = false;

    public AudioSource audio;

    public Slider audioVolume;
    // Start is called before the first frame update
    void Start()
    {
        tracking = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!slide && video.isPlaying)
        {
            tracking.value = (float)video.frame / video.frameCount;
        }

    }

    public void OnPointerDown(PointerEventData a)
    {
        slide = true;
    }

    public void OnPointerUp(PointerEventData a)
    {
        float frame = (float)tracking.value * video.frameCount;
        video.frame = (long)frame;
        slide = false;
        print(video.frame);
    }

    public void Volume()
    {
        audio.volume = audioVolume.value;
    }
}
