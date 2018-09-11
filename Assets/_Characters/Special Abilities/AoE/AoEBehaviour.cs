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
                var damageable = hit.collider.gameObject.GetComponent<IDamageable>();

                if (damageable != null)
                {
                    if (damageable.Equals(abilityUseParams.target) == false)
                    {
                        float damage = abilityUseParams.target.CalculateHitProbability(abilityUseParams.baseDamage, damageable);
                        AbilityUseParams aux = new AbilityUseParams(abilityUseParams.target, damage);
                        damageable.TakeDamage(aux.baseDamage + (config as AoEConfig).GetDamageToEachTarget());
                    }
                }
            }
        }
    }
}
