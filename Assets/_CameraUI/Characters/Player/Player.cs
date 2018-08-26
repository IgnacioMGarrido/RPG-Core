using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using RPG.CameraUI;
using RPG.Weapons;
using RPG.Core;

namespace RPG.Characters
{
    public class Player : MonoBehaviour, IDamageable
    {

        // Use this for initialization
        [SerializeField] int enemyLayer = 10;
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float damagePerHit = 10f;

        //temporarily serialized for debugging.
        [SerializeField] SpecialAbilityConfig ability1;


        [SerializeField] AnimatorOverrideController animatorOverrideController;
        Animator animator;

        [SerializeField] Weapon weaponInUse = null;

        GameObject currentTarget = null;
        RPGCursor cameraRaycaster;
        CharacterStats characterStats;

        [SerializeField] float currenthealthPoints = 100f;
        float lastHitTime = 0f;

        Energy playerEnergy;
        int energyPointsPerHit = 10;
        void Start()
        {
            playerEnergy = GetComponent<Energy>();
            InitializeCharacterStats();
            PutWeaponInHand();

            NotifyListeners();
            SetupRuntimeAnimator();
            ability1.AddComponent(gameObject);
        }

        private void NotifyListeners()
        {
            cameraRaycaster = Camera.main.GetComponent<RPGCursor>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        private void InitializeCharacterStats()
        {
            currenthealthPoints = maxHealthPoints;
            characterStats = GetComponent<CharacterStats>();
            if (characterStats != null)
            {
                maxHealthPoints = characterStats.Health;
                currenthealthPoints = maxHealthPoints;
                damagePerHit = characterStats.Damage;
                //weaponInUse.ActionSpeed += characterStats.ActionSpeedPercentage;
            }
        }

        private void SetupRuntimeAnimator()
        {
            
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController["DEFAULT ATTACK"] = weaponInUse.GetAttackAnimClip(); //remove const
        }

        void PutWeaponInHand()
        {

            GameObject weapon = Instantiate(weaponInUse.WeaponPrefab); //, weaponSlot.position, weaponSlot.rotation) as GameObject;
            GameObject dominantHandSocket = RequestDominantHand();
            weapon.transform.SetParent(dominantHandSocket.transform);
            weapon.transform.localPosition = weaponInUse.Grip.localPosition;
            weapon.transform.localRotation = weaponInUse.Grip.localRotation;

            SetWeaponModifiersToPlayer();
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

        // Update is called once per frame
        void Update()
        {

        }
        public void TakeDamage(float damage)
        {
            currenthealthPoints = Mathf.Clamp(currenthealthPoints - damage, 0f, maxHealthPoints);
        }
        //TODO: Refactor to reduce number of lines.

        void OnMouseOverEnemy(Enemy enemy) {
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy))
            {
                AttackTarget(enemy);
            }else if (Input.GetMouseButtonDown(1))
            {
                AttemptSpecialAbility1(enemy);
            }
        }

        private void AttemptSpecialAbility1(Enemy enemy)
        {
            if (playerEnergy.IsEnergyAvailable(ability1.GetEnergyCost())) { 
                playerEnergy.ConsumeEnergy(ability1.GetEnergyCost());
                GetComponent<ISpecialAbility>().Use();
            }
        }

        private void AttackTarget(Enemy target)
        {
            var enemyComponent = target;

            if (Time.time - lastHitTime > weaponInUse.ActionSpeed)
            {
                animator.SetTrigger("Attack");
                enemyComponent.TakeDamage(damagePerHit);
                lastHitTime = Time.time;
            }
        }

        private bool IsTargetInRange(Enemy target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= weaponInUse.MaxAttackRange;
        }

        public float healthAsPercentage
        {
            get
            {
                return currenthealthPoints / (float)maxHealthPoints;
            }

        }

        void SetWeaponModifiersToPlayer()
        { //TODO we may want to modify the player stats instead?
            damagePerHit = damagePerHit + damagePerHit * weaponInUse.AttackPercentageModifier;
            //actionSpeed = actionSpeed + actionSpeed * weaponInUse.SpeedPenaltyPercentageModifier;
        }



    }
}