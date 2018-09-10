using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {

        void Start()
        {
            print("Self heal behaviour attached to " + gameObject.name);
        }

        public override void Use(AbilityUseParams abilityUseParams)
        {
            HealTarget(abilityUseParams);
            PlayParticleEffect();
        }

        private void HealTarget(AbilityUseParams abilityUseParams)
        {
            print("Using Self Heal - " + (config as SelfHealConfig).GetHealAmount());
            
            abilityUseParams.target.TakeHeal(-(config as SelfHealConfig).GetHealAmount());
        }
    }
}
