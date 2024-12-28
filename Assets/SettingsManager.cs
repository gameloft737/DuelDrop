using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public int qualityLevel; 
    public void Awake(){
        SetQuality(qualityLevel);
    }
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20); 
    }
    public void SetQuality(int qualityIndex)
    {
        qualityLevel = qualityIndex;
        QualitySettings.SetQualityLevel(qualityLevel);
    }
}
