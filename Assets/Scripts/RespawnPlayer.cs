using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPlayer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerManager.LocalPlayer.Respawn(new UnityEngine.InputSystem.InputAction.CallbackContext());
    }
}
