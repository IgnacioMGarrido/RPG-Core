using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float projectileSpeed = 10f;
    public float damageCaused = 10f;

    private void OnTriggerEnter(Collider other)
    {
        Component damagable = other.gameObject.GetComponent(typeof(IDamageable));
        if (damagable) {
            (damagable as IDamageable).TakeDamage(damageCaused);
        }
    }
}
