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

        // Update is called once per frame
        void Update()
        {

        }

        public void Use(AbilityUseParams abilityUseParams)
        {
            print("Using Power Attack extra damage - " + config.GetExtraDamage() + abilityUseParams.baseDamage);
            abilityUseParams.target.TakeDamage(abilityUseParams.baseDamage + config.GetExtraDamage());
        }
    }
}
