using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    Light _light;
    [SerializeField] private int _partOfLevel;
    [SerializeField] private int _lightOrder;
    private void Awake()
    {
        _light = GetComponent<Light>();
        if(_light != null )
            _light.enabled= false;
        else
            gameObject.SetActive(false);
        GameManager.Instance.NotifyLights.AddListener(ChangeLightState);
    }

    private void ChangeLightState(int levelPart,int lightOrder)
    {
        bool disableGameobject = _light == null;
        if(levelPart != _partOfLevel)
        {
            if (disableGameobject)
                gameObject.SetActive(false);
            else
                _light.enabled = false;
            return;
        }

        if(lightOrder == _lightOrder)
        {
            if (disableGameobject)
                gameObject.SetActive(true);
            else
                _light.enabled = true;
        }
    }
}
