using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Weapons
{
    [CreateAssetMenu(menuName = ("RPG/Weapon"))]
    public class Weapon : ScriptableObject {

    [SerializeField] GameObject weaponPrefab;
    [SerializeField] AnimationClip attackAnimation;
    [SerializeField] Transform grip;

    [SerializeField] float attackPercentageModifier = 0.0f;
    [SerializeField] float speedPenaltyPercentageModifier = 0.0f;

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
}
}