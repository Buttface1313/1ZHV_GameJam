using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VolumeManager : MonoBehaviour
{
    [Range(0f, 1f)]
    public float MusicVolumme = 1;
    [Range(0f, 1f)]
    public float MasterVolume = 1;

    public UnityEvent<float,float> VolumeChanged;
}
