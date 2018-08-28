using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Weapons
{
    [CreateAssetMenu(menuName = ("RPG/Weapon"))]
    public class Weapon : ScriptableObject {

        [SerializeField] bool isTwoHanded;
        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAnimation;
        [SerializeField] Transform grip;
        [SerializeField] float damageModifier = 0.15f;
        [SerializeField] float actionSpeedModifier = 0.5f;
        [SerializeField] float maxAttackRange = 1.5f;

        public GameObject WeaponPrefab
        {
            get
            {
                return weaponPrefab;
            }

            private set
            {
                weaponPrefab = value;
            }
        }

        public float DamageModifier
        {
            get
            {
                return damageModifier;
            }

            private set
            {
                damageModifier = value;
            }
        }
        public Transform Grip
        {
            get
            {
                return grip;
            }

            set
            {
                grip = value;
            }
        }

        public float ActionSpeedModifier
        {
            get
            {
                return actionSpeedModifier;
            }

            set
            {
                actionSpeedModifier = value;
            }
        }

        public float MaxAttackRange
        {
            get
            {
                return maxAttackRange;
            }

            set
            {
                maxAttackRange = value;
            }
        }

        public AnimationClip GetAttackAnimClip()
        {
            RemoveAnimationEvents();
            return attackAnimation;
        }

        //So that asset packs cannot caouse crashes.
        private void RemoveAnimationEvents()
        {
            attackAnimation.events = new AnimationEvent[0];
        }
    }
}