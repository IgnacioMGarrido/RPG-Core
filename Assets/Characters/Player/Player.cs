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
       // [SerializeField] float baseDamage = 10f;

        //temporarily serialized for debugging.
        [SerializeField] SpecialAbility[] abilities;


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
            foreach (SpecialAbility ability in abilities)
            {
                ability.AttachComponentTo(gameObject);
                ModifyAoERadius(ability);
            }
        }

        private void ModifyAoERadius(SpecialAbility ability)
        {
            if (ability.GetType() == typeof(AoEBehaviour)) {
                //TODO: check radius
            }
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
                maxHealthPoints = characterStats.GetHealth();
                currenthealthPoints = maxHealthPoints;
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
            //healing
            if (Input.GetKeyDown(KeyCode.Alpha1)) //Self Healing
            {

                AttemptSpecialAbility(1, gameObject.GetComponent<Player>(), characterStats.GetHealing());
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))// AoEDamage
            {
                AttemptSpecialAbility(2, gameObject.GetComponent<Player>(), characterStats.GetDamage());
            }
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
            }
            else if (Input.GetMouseButtonDown(1))
            {
                AttemptSpecialAbility(0, enemy, characterStats.GetDamage());
            }
        }

        private void AttemptSpecialAbility(int abilityIndex, IDamageable target, float amount)
        {
            if (playerEnergy.IsEnergyAvailable(abilities[abilityIndex].GetEnergyCost())) { 
                playerEnergy.ConsumeEnergy(abilities[abilityIndex].GetEnergyCost());
                
                abilities[abilityIndex].Use(new AbilityUseParams(target, amount));
            }
        }

        private void AttackTarget(Enemy target)
        {
            var enemyComponent = target;

            if (Time.time - lastHitTime > characterStats.GetActionSpeed())
            {
                animator.SetTrigger("Attack");
                enemyComponent.TakeDamage(characterStats.GetDamage());
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
            if (characterStats != null)
            {
                characterStats.SetActionSpeed(weaponInUse.ActionSpeedModifier);
                characterStats.SetDamage(weaponInUse.DamageModifier);
            }
        }



    }
}