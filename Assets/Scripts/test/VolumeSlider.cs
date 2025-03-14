using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VolumeSlider : MonoBehaviour
{
    public TextMeshProUGUI VolumeNum;
    public void OnVolumeChanged(float value)
    {
        int intValue = Mathf.RoundToInt(value * 100);
        VolumeNum.text = intValue.ToString();

        TownSEManager.instance.SetMasterVolume(value);
        LoopSEManager.instance.SetMasterVolume(value);
        PlayerLoopSEManager.instance.SetMasterVolume(value);
        SEManager.instance.SetMasterVolume(value);
    }
}
