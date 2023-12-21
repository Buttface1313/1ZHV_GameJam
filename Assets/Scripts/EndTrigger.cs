using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTrigger : MonoBehaviour
{
    [SerializeField] LevelEndScreen endScreen;
    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.SaveTime();
        endScreen.gameObject.SetActive(true);
    }
}
