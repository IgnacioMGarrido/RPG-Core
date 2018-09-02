using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
namespace RPG.Characters
{
    public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility
    {
        PowerAttackConfig config;

        public void SetConfig(PowerAttackConfig configToSet) {
            config = configToSet;
        }

        // Use this for initialization
        void Start()
        {
            print("Power Attack behaviour attached to " + gameObject.name);
        }

        private void PlayParticleEffect()
        {
            if (config.GetParticlePrefab() != null)
            {
                GameObject go = Instantiate(config.GetParticlePrefab(), transform);
                ParticleSystem myParticleSystem = null;
                myParticleSystem = go.GetComponent<ParticleSystem>();

                // ParticleSystem.ShapeModule shapeModule = myParticleSystem.shape;
                // shapeModule.radius = radius;

                myParticleSystem.Play();
                Destroy(go, 3);

            }
        }
        public void Use(AbilityUseParams abilityUseParams)
        {
            DealDamage(abilityUseParams);
            PlayParticleEffect();
        }

        private void DealDamage(AbilityUseParams abilityUseParams)
        {
            print("Using Power Attack extra damage - " + config.GetExtraDamage() + " + " + abilityUseParams.baseDamage);
            abilityUseParams.target.TakeDamage(abilityUseParams.baseDamage + config.GetExtraDamage());
        }
    }
}
