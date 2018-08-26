using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
namespace RPG.Characters
{
    public class SelfHealBehaviour : MonoBehaviour, ISpecialAbility
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

        // Update is called once per frame
        void Update()
        {

        }

        public void Use(AbilityUseParams abilityUseParams)
        {
            print("Using Self Heal - " + config.GetHealAmount());
            abilityUseParams.target.TakeDamage(-config.GetHealAmount());
        }
    }
}
