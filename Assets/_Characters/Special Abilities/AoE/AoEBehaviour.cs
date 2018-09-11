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
        public override void Use(AbilityUseParams abilityUseParams)
        {
            DealRadialDamage(abilityUseParams);
            PlayParticleEffect();
            PlayAbilitySound();
        }
        private void DealRadialDamage(AbilityUseParams abilityUseParams)
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
                    if (healthSystem.Equals(abilityUseParams.target) == false)
                    {
                        //TODO fix this to do the same as be4 when the architecture is completed
                        float damage = 10;// abilityUseParams.target.CalculateHitProbability(abilityUseParams.baseDamage, healthSystem);
                        AbilityUseParams aux = new AbilityUseParams(abilityUseParams.target, damage);
                        healthSystem.TakeDamage(aux.baseDamage + (config as AoEConfig).GetDamageToEachTarget());
                    }
                }
            }
        }
    }
}
