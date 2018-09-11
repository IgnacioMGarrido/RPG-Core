using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public class AoEBehaviour : AbilityBehaviour
    {
        float radius;
        public void SetRadiusModifier(float abilityRadiusModifier)
        {
            radius = (config as AoEConfig).GetRadius() * abilityRadiusModifier;
        }
        public override void Use(HealthSystem hs)
        {
            DealRadialDamage(hs);
            PlayParticleEffect();
            PlayAbilitySound();
        }
        private void DealRadialDamage(HealthSystem hs)
        {
            RaycastHit[] hits = Physics.SphereCastAll(
                transform.position,
                radius,
                Vector3.up,
                radius
            );
            foreach (RaycastHit hit in hits)
            {
                var healthSystem = hit.collider.gameObject.GetComponent<HealthSystem>();

                if (healthSystem != null)
                {
                    if (healthSystem.Equals(hs) == false)
                    {
                        //TODO fix this to do the same as be4 when the architecture is completed
                        healthSystem.TakeDamage((config as AoEConfig).GetDamageToEachTarget());
                    }
                }
            }
        }
    }
}
