using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

using UnityEngine;
using System;

namespace RPG.Characters
{
    public class WeaponSystem : MonoBehaviour
    {
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";
        const string ATTACK_TRIGGER = "Attack";


        [Header("Weapon")]
        [SerializeField] Weapon currentWeaponConfig = null;

        GameObject weaponGameObject;
        CharacterStats characterStats;
        Character character;
        Animator animator;

        HealthSystem currentTarget;
        float lastHitTime = 0f;

        // Use this for initialization
        void Start()
        {
            characterStats = GetComponent<CharacterStats>();
            character = GetComponent<Character>();
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
            if (character.GetOverrideController() != null)
            {
                animator = GetComponent<Animator>();
                AnimatorOverrideController overrideController = character.GetOverrideController();
                animator.runtimeAnimatorController = overrideController;
                overrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip(); //remove const
            }
            else
            {
                Debug.Break();
                Debug.LogError("Please put an animator override controller in " + this.name);
            }
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

        public void AttackTarget(HealthSystem targetHealthSystem)
        {
            currentTarget = targetHealthSystem;
            print("Attacking " + currentTarget.name);
            StartCoroutine(AttackRepeatidly());
        }

        private IEnumerator AttackRepeatidly() {
            while (GetComponent<HealthSystem>().GetIsAlive() == true && currentTarget.GetIsAlive())
            {
                float hitPeriod = characterStats.GetActionSpeed();
                float waitTime = hitPeriod * character.GetAnimatorSpeedMultiplier();

                bool hitAgain = Time.time - lastHitTime > waitTime;
                if (hitAgain)
                {
                    AttackTargetOnce();
                    lastHitTime = Time.time;
                }
                yield return new WaitForSeconds(waitTime);
            }
        }

        private void AttackTargetOnce()
        {
            transform.LookAt(currentTarget.transform);
            animator.SetTrigger(ATTACK_TRIGGER);
            float damageDelay = 1.0f;
            SetupAttackAnimation();
            StartCoroutine(DamageAfterDelay(damageDelay));
        }

        IEnumerator DamageAfterDelay(float damageDelay)
        {
            yield return new WaitForSeconds(damageDelay);
            currentTarget.TakeDamage(CalculateHitProbability(characterStats.GetDamage(), currentTarget));
        }

        public float CalculateHitProbability(float damage, HealthSystem targetToHit)
        {
            int score = UnityEngine.Random.Range(1, 101);

            float damageDealerNewAccuracy = GetComponent<CharacterStats>().GetAccuracy() - targetToHit.GetComponent<CharacterStats>().GetDeflection();
            float attackRoll = score + damageDealerNewAccuracy;
            print("------------------------------------------------------------------------------");
            print("Attack Roll: " + score + "(score) + " + damageDealerNewAccuracy + " (Player Accuracy - Enemy Deflection) " + " = " + attackRoll);

            if (attackRoll > 25 && attackRoll <= 50)
            {
                damage = damage / 2;
                print("This hit was a GRAZE. Damage/2 = " + damage);
            }
            else if (attackRoll > 0 && attackRoll < 25)
            {
                damage = 0;
                print("This hit was a MISS. Damage =" + damage);
            }
            else if (attackRoll > 100)
            {
                damage = damage * 1.25f;
                print("This hit was a CRIT HIT. Damage * 1.25 = " + damage);

            }
            else
            {
                print("This hit was a NORMAL HIT. Damage = " + damage);
            }

            return damage;
        }
    }
}