using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        Component damagable = other.gameObject.GetComponent(typeof(IDamageable));
        if (damagable) {
            (damagable as IDamageable).TakeDamage(10);
        }
    }
}
