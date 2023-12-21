using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BounceSound : MonoBehaviour
{
    [SerializeField] AudioSource m_AudioSource;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 4)
            m_AudioSource.Play();
    }
    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.volume = 0.05f;
    }
}
