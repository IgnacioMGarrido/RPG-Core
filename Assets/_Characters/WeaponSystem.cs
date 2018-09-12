using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

using UnityEngine;

namespace RPG.Characters
{
    public class WeaponSystem : MonoBehaviour
    {
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";


        [Header("Weapon")]
        [SerializeField] Weapon currentWeaponConfig = null;
        [SerializeField] AnimatorOverrideController animatorOverrideController;

        GameObject weaponGameObject;
        CharacterStats characterStats;
        Animator animator;
        // Use this for initialization
        void Start()
        {
            characterStats = GetComponent<CharacterStats>();
            animator = GetComponent<Animator>();
            SetupAttackAnimation();
            if (currentWeaponConfig != null)
                PutWeaponInHand(currentWeaponConfig);
        }


        public void PutWeaponInHand(Weapon weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.WeaponPrefab;
            GameObject dominantHandSocket = RequestDominantHand();
            Destroy(weaponGameObject);
            weaponGameObject = Instantiate(weaponToUse.WeaponPrefab, dominantHandSocket.transform); //, weaponSlot.position, weaponSlot.rotation) as GameObject;
            weaponGameObject.transform.localPosition = weaponToUse.Grip.localPosition;
            weaponGameObject.transform.localRotation = weaponToUse.Grip.localRotation;

            SetWeaponModifiersToCharacter();
            //TODO: Maybe do this everytime we attack instead??
            SetupAttackAnimation();
        }

        void SetWeaponModifiersToCharacter()
        {
            if (characterStats != null)
            {
                characterStats.SetActionSpeed(currentWeaponConfig.ActionSpeedModifier);
                characterStats.SetDamage(currentWeaponConfig.DamageModifier);
            }
        }

        private void SetupAttackAnimation()
        {

            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip(); //remove const
        }

        private GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numDominantHands = dominantHands.Length;
            //Handle 0 hands
            Assert.IsFalse(numDominantHands <= 0, "No dominant hand found on Player. Please add one");
            //handle more than one hand
            Assert.IsFalse(numDominantHands > 1, "Multiple Dominant hand Scripts on player, pleasse remove one");

            return dominantHands[0].gameObject;

        }

        public Weapon GetCurrentWeaponConfig()
        {
            return currentWeaponConfig;
        }

    }
}