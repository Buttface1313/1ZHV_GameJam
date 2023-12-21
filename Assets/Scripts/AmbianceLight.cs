using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbianceLight : MonoBehaviour
{
    Light _light;
    float targetIntensity;
    [SerializeField] private int _partOfLevel;
    [SerializeField] private int _countOfLights = 1;
    private void Awake()
    {
        _light = GetComponent<Light>();
        targetIntensity = _light.intensity;
        _light.intensity = 0;
        GameManager.Instance.NotifyLights.AddListener(ChangeLightState);
    }

    private void ChangeLightState(int levelPart, int lightOrder)
    {
        bool disableGameobject = _light == null;
        if (levelPart != _partOfLevel)
        {
            _light.enabled = false;
            return;
        }
        if (lightOrder > _countOfLights)
            return;
        _light.intensity = targetIntensity * lightOrder/(float)_countOfLights;
    }
}
