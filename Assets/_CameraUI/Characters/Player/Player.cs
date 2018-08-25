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


        [SerializeField] AnimatorOverrideController animatorOverrideController;
        Animator animator;

        [SerializeField] Weapon weaponInUse = null;

        GameObject currentTarget = null;
        CameraRaycaster cameraRaycaster;
        CharacterStats characterStats;

        [SerializeField] float currenthealthPoints = 100f;
        float lastHitTime = 0f;
        void Start()
        {
            PutWeaponInHand();
            InitializeCharacterStats();

            NotifyListeners();
            SetupRuntimeAnimator();
        }

        private void NotifyListeners()
        {
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
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
        void OnMouseClick(RaycastHit raycastHit, int layerHit)
        {
            //TODO: check dependencies.
            if (layerHit == enemyLayer)
            {
                var enemy = raycastHit.collider.gameObject;
                //Check enemy is in range.
                if (IsTargetInRange(enemy)) {
                    AttackTarget(enemy);
                }
            }
        }

        private void AttackTarget(GameObject target)
        {
            currentTarget = target;
            var enemyComponent = currentTarget.GetComponent<Enemy>();

            if (Time.time - lastHitTime > weaponInUse.ActionSpeed)
            {
                animator.SetTrigger("Attack");
                enemyComponent.TakeDamage(damagePerHit);
                lastHitTime = Time.time;
            }
        }

        private bool IsTargetInRange(GameObject target)
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
            //damagePerHit = damagePerHit + damagePerHit * weaponInUse.AttackPercentageModifier;
            //actionSpeed = actionSpeed + actionSpeed * weaponInUse.SpeedPenaltyPercentageModifier;
        }



    }
}