using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private AudioSource _source;
    private float _userSelectedVolume;
    private float _actualMaxVolume;
    bool _playingAtMax = false;
    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        SettingsManager.Instance.OnMusicVolumeChanged.AddListener(ChangeMusic);
        _actualMaxVolume = _source.volume;
        _userSelectedVolume = SettingsManager.Instance.MusicVolume;
        StartCoroutine(StartMusic(10));
    }
    private void OnDestroy()
    {
        SettingsManager.Instance.OnMusicVolumeChanged.RemoveListener(ChangeMusic);
    }
    private void ChangeMusic(float newVol)
    {
        if (_playingAtMax)
        {
            _source.volume = _actualMaxVolume * newVol;
        }
        _userSelectedVolume = newVol;
    }
    private IEnumerator StartMusic(float timeToStart)
    {
        float startTime = Time.time;
        while (!_playingAtMax)
        {
            _source.volume = ((Time.time - startTime) / timeToStart)*_userSelectedVolume * _actualMaxVolume;
            _playingAtMax=(Time.time - startTime) >= timeToStart;
            yield return null;
        }
    }
}
