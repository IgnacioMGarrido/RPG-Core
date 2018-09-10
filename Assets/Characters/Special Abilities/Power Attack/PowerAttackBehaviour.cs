using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
namespace RPG.Characters
{
    public class PowerAttackBehaviour : AbilityBehaviour
    {

        // Use this for initialization
        void Start()
        {
            print("Power Attack behaviour attached to " + gameObject.name);
        }

        public override void Use(AbilityUseParams abilityUseParams)
        {
            DealDamage(abilityUseParams);
            PlayParticleEffect();
        }

        private void DealDamage(AbilityUseParams abilityUseParams)
        {
            print("Using Power Attack extra damage - " + (config as PowerAttackConfig).GetExtraDamage() + " + " + abilityUseParams.baseDamage);
            abilityUseParams.target.TakeDamage(abilityUseParams.baseDamage + (config as PowerAttackConfig).GetExtraDamage());
        }
    }
}
