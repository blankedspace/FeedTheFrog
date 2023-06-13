using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    void Start()
    {
        GetComponent<Slider>().value = SoundMaster._volume;
        GetComponent<Slider>().onValueChanged.AddListener(SoundMaster.ChangeVolume);
    }

}
