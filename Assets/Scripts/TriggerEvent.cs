using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    [SerializeField] UnityEvent Triggered;

    private void OnTriggerEnter(Collider other)
    {
        Triggered?.Invoke();
    }
}
