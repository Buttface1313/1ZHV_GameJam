using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
    [SerializeField] private Slider SFXVolume;
    [SerializeField] private Slider MusicVolume;
    [SerializeField] private Slider MouseSensitivity;

    private void Awake()
    {
        SFXVolume.value = SettingsManager.Instance.SFXVol;
        MusicVolume.value = SettingsManager.Instance.SFXVol;
        MouseSensitivity.value = SettingsManager.Instance.MouseSensitivity;

        SFXVolume.onValueChanged.AddListener(SettingsManager.Instance.ChangeSFXVol);
        MusicVolume.onValueChanged.AddListener(SettingsManager.Instance.ChangeMusicVol);
        MouseSensitivity.onValueChanged.AddListener(SettingsManager.Instance.ChangeMouseSens);
    }
    private void OnDestroy()
    {
        SFXVolume.onValueChanged.RemoveAllListeners();
        MusicVolume.onValueChanged.RemoveAllListeners();
        MouseSensitivity.onValueChanged.RemoveAllListeners();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
