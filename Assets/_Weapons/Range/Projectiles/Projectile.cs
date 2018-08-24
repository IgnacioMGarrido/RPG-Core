using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;

namespace RPG.Weapons
{
    public class Projectile : MonoBehaviour
    {

        const float DESTROY_DELAY = 0.05f;

        [SerializeField] private float projectileSpeed = 10f;
        [SerializeField] float damageCaused = 10f;
        [SerializeField] GameObject shooter;

        private void OnTriggerEnter(Collider collision) //Maybe change this to actual collision instead of triggers
        {
            if (shooter && collision.gameObject.layer != shooter.layer)
            {
                DamageIfDamageable(collision);
            }
        }

        private void DamageIfDamageable(Collider collision)
        {
            Component damagable = collision.gameObject.GetComponent(typeof(IDamageable));
            if (damagable && damagable.gameObject.layer != shooter.layer)
            {
                (damagable as IDamageable).TakeDamage(damageCaused);
            }
            Destroy(this.gameObject, DESTROY_DELAY);
        }

        public float getDefaultLaunchSpeed()
        {
            return projectileSpeed;
        }
        public void SetDamage(float damage)
        {
            damageCaused = damage;
        }

        public void SetShooter(GameObject _Shooter)
        {
            this.shooter = _Shooter;
        }
    }
}