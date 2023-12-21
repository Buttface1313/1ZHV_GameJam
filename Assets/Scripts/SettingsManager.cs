using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    public float SFXVol = 1f;
    public float MusicVolume = 1f;
    public float MouseSensitivity = 0.1f;

    public UnityEvent<float> OnMouseSensitivityChanged;
    public UnityEvent<float> OnSFXVolumeChanged;
    public UnityEvent<float> OnMusicVolumeChanged;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (Instance != null)
            Debug.LogError("TOO MANY SETTINGS MANAGERS");
        Instance = this;
        //should add some playerprefs
    }
    public void ChangeSFXVol(float vol)
    {
        SFXVol = vol;
        OnSFXVolumeChanged?.Invoke(vol);
    }
    public void ChangeMusicVol(float vol)
    {
        MusicVolume = vol;
        OnMusicVolumeChanged?.Invoke(vol);
    }
    public void ChangeMouseSens(float sensitivity)
    {
        MouseSensitivity = sensitivity;
        OnMouseSensitivityChanged?.Invoke(sensitivity);
    }
}
