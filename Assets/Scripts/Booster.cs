using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{
    [SerializeField] PlayerManager.BoosterTypes boostType = PlayerManager.BoosterTypes.SpeedBoost;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
        if (other.TryGetComponent(out PlayerManager player)){
            Debug.Log("Boosted");
            player.AddBooster(boostType);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out PlayerManager player))
        {
            Debug.Log("Unboosted");
            player.RemoveCounter(boostType);
        }
    }
}
