using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {
        SelfHealConfig config;

        public void SetConfig(SelfHealConfig configToSet) {
            config = configToSet;
        }

        // Use this for initialization
        void Start()
        {
            print("Self heal behaviour attached to " + gameObject.name);
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
        public override void Use(AbilityUseParams abilityUseParams)
        {
            HealTarget(abilityUseParams);
            PlayParticleEffect();
        }

        private void HealTarget(AbilityUseParams abilityUseParams)
        {
            print("Using Self Heal - " + config.GetHealAmount());
            
            abilityUseParams.target.TakeHeal(-config.GetHealAmount());
        }
    }
}
