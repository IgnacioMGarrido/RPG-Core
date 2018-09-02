using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
    public class AoEBehaviour : MonoBehaviour, ISpecialAbility
    {
        AoEConfig config;
        ParticleSystem myParticleSystem = null;
        float radius;
        public void SetConfig(AoEConfig configToSet) {
            config = configToSet;
            radius = config.GetRadius();
           
        }

        // Use this for initialization
        void Start()
        {
            print("AoE behaviour attached to " + gameObject.name);
        }
        public void SetRadiusModifier(float abilityRadiusModifier)
        {
            radius = config.GetRadius() * abilityRadiusModifier;
        }
        public void Use(AbilityUseParams abilityUseParams)
        {

            DealRadialDamage(abilityUseParams);
            PlayParticleEffect();
        }

        private void PlayParticleEffect()
        {
            if (config.GetParticlePrefab() != null){
               GameObject go = Instantiate(config.GetParticlePrefab(), transform);
                myParticleSystem = go.GetComponent<ParticleSystem>();

               // ParticleSystem.ShapeModule shapeModule = myParticleSystem.shape;
               // shapeModule.radius = radius;

                myParticleSystem.Play();
                Destroy(go, 3);

            }
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
                        damageable.TakeDamage(aux.baseDamage + config.GetDamageToEachTarget());
                    }
                }
            }
        }
    }
}
