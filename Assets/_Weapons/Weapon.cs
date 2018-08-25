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
        [SerializeField] float attackPercentageModifier = 0.15f;
        [SerializeField] float actionSpeed = 0.5f;
        [SerializeField] float maxAttackRange = 1.5f;

  //      [SerializeField] float attackPercentageModifier = 0.0f;
  //      [SerializeField] float speedPenaltyPercentageModifier = 0.0f;

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

        public float AttackPercentageModifier
        {
            get
            {
                return attackPercentageModifier;
            }

            private set
            {
                attackPercentageModifier = value;
            }
        }
/*
        public float SpeedPenaltyPercentageModifier
        {
            get
            {
                return speedPenaltyPercentageModifier;
            }

            private set
            {
                speedPenaltyPercentageModifier = value;
            }
        }
*/
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

        public float ActionSpeed
        {
            get
            {
                return actionSpeed;
            }

            set
            {
                actionSpeed = value;
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