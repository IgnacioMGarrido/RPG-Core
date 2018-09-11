using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using RPG.CameraUI;
using RPG.Core;

namespace RPG.Characters
{
    public class Player : MonoBehaviour
    {
 
        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";

        Enemy target;
        // Use this for initialization

        [Header("Special Abilities")]
        
        //temporarily serialized for debugging.


        [Header("Animator")]
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        Animator animator;

        [Header("Weapon")]
        [SerializeField] Weapon currentWeaponConfig = null;
        GameObject weaponGameObject;



        GameObject currentTarget = null;
        RPGCursor cameraRaycaster;
        CharacterStats characterStats;

        float lastHitTime = 0f;

        SpecialAbilities playerAbilities;
        bool isDead = false;
  
        void Start()
        {
            playerAbilities = GetComponent<SpecialAbilities>();
            characterStats = GetComponent<CharacterStats>();
            PutWeaponInHand(currentWeaponConfig);

            NotifyListeners();
            SetupAttackAnimation();

            ModifyAoERadius();
        }



        private void ModifyAoERadius()
        {
            AoEBehaviour[] AoEAbilities = GetComponents<AoEBehaviour>();
            if (AoEAbilities.Length > 0) {
                foreach (AoEBehaviour aoEability in AoEAbilities) {
                   aoEability.SetRadiusModifier(characterStats.GetAoEModifier());
                }
            }
        }

        private void NotifyListeners()
        {
            cameraRaycaster = Camera.main.GetComponent<RPGCursor>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        private void SetupAttackAnimation()
        {
            
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip(); //remove const
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

            SetWeaponModifiersToPlayer();
            //TODO: Maybe do this everytime we attack instead??
            SetupAttackAnimation();
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
            var healthAsPercentage = GetComponent<HealthSystem>().HealthAsPercentage;
            if (healthAsPercentage > Mathf.Epsilon)
            {
                ScanForAbilityKey();
            }
        }

        private void ScanForAbilityKey()
        {
            //TODO: abstract this to each class ( Maybe the ability is the one that needs to access the character stats )
            //healing
            if (Input.GetKeyDown(KeyCode.Alpha1)) //Self Healing
            {
                playerAbilities.AttemptSpecialAbility(1, GetComponent<HealthSystem>(), characterStats.GetHealing()); 
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))// AoEDamage
            {
                playerAbilities.AttemptSpecialAbility(2, GetComponent<HealthSystem>(), characterStats.GetDamage());

            }
        }


        void OnMouseOverEnemy(Enemy enemyToSet) {

            this.target = enemyToSet;
            if (isDead == false)
            {
                if (Input.GetMouseButton(0) && IsTargetInRange(this.target as Enemy))
                {
                    AttackTarget();
                }
                else if (Input.GetMouseButtonDown(1) && IsTargetInRange(enemyToSet))
                {
                    playerAbilities.AttemptSpecialAbility(0, this.target.GetComponent<HealthSystem>(), characterStats.GetDamage());
                }
            }
        }

        private void AttackTarget()
        {

            if (Time.time - lastHitTime > characterStats.GetActionSpeed())
            {

                animator.SetTrigger(ATTACK_TRIGGER);
                float hitValue = CalculateHitProbability(characterStats.GetDamage(), target.GetComponent<HealthSystem>());
                //target.TakeDamage(hitValue);
                lastHitTime = Time.time;
            }
        }
        //TODO: Clear this mess!
        public float CalculateHitProbability(float damage, HealthSystem enemy)
        {
            int score = Random.Range(1, 101);
            
            float damageDealerNewAccuracy = GetComponent<CharacterStats>().GetAccuracy() - enemy.GetComponent<CharacterStats>().GetDeflection();
            float attackRoll = score + damageDealerNewAccuracy;
            print("------------------------------------------------------------------------------");
            print("Attack Roll: " + score + "(score) + " + damageDealerNewAccuracy + " (Player Accuracy - Enemy Deflection) " + " = " + attackRoll);
            
            if (attackRoll > 25 && attackRoll <= 50)
            {
                damage = damage / 2;
                print("This hit was a GRAZE. Damage/2 = " + damage);
            }
            else if (attackRoll > 0 && attackRoll < 25) {
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
        private bool IsTargetInRange(Enemy target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= currentWeaponConfig.MaxAttackRange;
        }


        void SetWeaponModifiersToPlayer()
        { 
            if (characterStats != null)
            {
                characterStats.SetActionSpeed(currentWeaponConfig.ActionSpeedModifier);
                characterStats.SetDamage(currentWeaponConfig.DamageModifier);
            }
        }

        public bool GetIsDead() {
            return this.isDead;
        }


    }
}