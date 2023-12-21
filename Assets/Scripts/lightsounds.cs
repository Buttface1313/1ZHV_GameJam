using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightsounds : MonoBehaviour
{
    private float _actualMaxVolume;
    private AudioSource _source;
    void Awake()
    {
        _source = GetComponent<AudioSource>();
        SettingsManager.Instance.OnSFXVolumeChanged.AddListener(ChangeVolume);
        _actualMaxVolume = _source.volume;
    }

    public void PlaySound()
    {
        _source.Play();
    }

    void ChangeVolume(float volume)
    {
        _source.volume = volume * _actualMaxVolume;
    }
}
