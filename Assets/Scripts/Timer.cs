using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    PlayerManager _playerManager;
    [SerializeField]TextMeshProUGUI _text;
    private void Awake()
    {
        _playerManager = PlayerManager.LocalPlayer;
    }
    private void Update()
    {
        GameManager.Instance.GetCurrentTime(out float startTime,out float totalTime);
        if (startTime != 0f)
        {
            _text.text = $"Level timer:\n{getFormatedTime(Time.time - startTime)}\nTotal time:\n{getFormatedTime(Time.time - totalTime)}" ;
        }
        else if(totalTime != 0f)
        {
            _text.text = $"Level timer:\n0:00:00\nTotal time:\n{getFormatedTime(Time.time - totalTime)}";
        }
        else
        {
            _text.text = $"Level timer:\n0:00:00\nTotal time:\n0:00:00";
        }
    }

    string getFormatedTime(float currentTime)
    {
        int minutes = (int)currentTime / 60;
        string seconds = ((int)currentTime % 60).ToString("D2");
        string miliseconds = ((int)(currentTime * 100) % 100).ToString("D2");
        return $"{minutes}:{seconds}:{miliseconds}";
    }
}
