using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Core;
namespace RPG.Characters
{

    public struct AbilityUseParams
    {
        public IDamageable target;
        public float baseDamage;

        public AbilityUseParams(IDamageable _target, float _baseDamage) {
            this.target = _target;
            this.baseDamage = _baseDamage;
        }
    }
    public abstract class SpecialAbility : ScriptableObject
    {
        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;

        protected ISpecialAbility behaviour;
        [SerializeField] AnimationClip abilityAnimation;

        abstract public void AttachComponentTo(GameObject gameObjectToAttachTo);
        public void Use(AbilityUseParams abilityUseParams) {
            behaviour.Use(abilityUseParams);
        }
        public float GetEnergyCost() {
            return energyCost;
        }
    }
    public interface ISpecialAbility
    {
        void Use(AbilityUseParams abilityUseParams);
    }
}
