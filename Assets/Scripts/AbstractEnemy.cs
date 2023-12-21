using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractEnemy : MonoBehaviour, IDamageable
{
    public int Health;
    public void Damage(int damage) {
        Health -= damage;
        if (Health <= 0) {
            Kill();
        }
    }

    public void Kill() {
        Destroy(gameObject);
    }
}
