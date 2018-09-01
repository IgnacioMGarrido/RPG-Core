using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using RPG.CameraUI;
using RPG.Weapons;
using RPG.Core;

namespace RPG.Characters
{
    public class Player : MonoBehaviour, IDamageable
    {
        const string DEATH_TRIGGER = "Death";
        const string ATTACK_TRIGGER = "Attack";

        // Use this for initialization
        [SerializeField] float maxHealthPoints = 100f;
        float currenthealthPoints = 100f;

        [Header("Special Abilities")]
        
        //temporarily serialized for debugging.
        [SerializeField] SpecialAbility[] abilities;

        [Header("Animator")]
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        Animator animator;

        [Header("Weapon")]
        [SerializeField] Weapon weaponInUse = null;

        [Header("Audio")]
        AudioSource audioSource = null;
        [SerializeField] AudioClip[] hitSounds;
        [SerializeField] AudioClip[] deathSounds;


        GameObject currentTarget = null;
        RPGCursor cameraRaycaster;
        CharacterStats characterStats;

        float lastHitTime = 0f;

        Energy playerEnergy;
        int energyPointsPerHit = 10;
        bool isDead = false;
  
        void Start()
        {
            playerEnergy = GetComponent<Energy>();
            audioSource = GetComponent<AudioSource>();

            InitializeCharacterStats();
            PutWeaponInHand();

            NotifyListeners();
            SetupRuntimeAnimator();
            AttachAbilitiesToPlayer();


            ModifyAoERadius();
        }

        private void AttachAbilitiesToPlayer()
        {
            foreach (SpecialAbility ability in abilities)
            {
                ability.AttachComponentTo(gameObject);
            }
        }

        private void ModifyAoERadius()
        {
            AoEBehaviour[] AoEAbilities = GetComponents<AoEBehaviour>();
            if (AoEAbilities.Length > 0) {
                foreach (AoEBehaviour AoEability in AoEAbilities) {
                    AoEability.SetRadiusModifier(characterStats.GetAoEModifier());
                }
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
            if (isDead == false)
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
        }
        public void TakeDamage(float damage)
        {
            if (currenthealthPoints - damage > 0)
            {
                PlayRandomAudioClip(hitSounds);
                ReduceHealth(damage);
            }
            else
            {
                ReduceHealth(damage);
                //Player Dies
                if (isDead == false)
                {
                    PlayRandomAudioClip(deathSounds);
                    StartCoroutine(KillPlayer());
                }
            }
        }
        private void PlayRandomAudioClip(AudioClip[] clips)
        {
            audioSource.clip = clips[(int)Random.Range(0, clips.Length)];
            if(!audioSource.isPlaying)
                audioSource.Play();
        }
        IEnumerator KillPlayer()
        {
            isDead = true;
            animator.SetTrigger(DEATH_TRIGGER);
            float duration = audioSource.clip.length > animator.GetCurrentAnimatorClipInfo(0).Length ? audioSource.clip.length : animator.GetCurrentAnimatorClipInfo(0).Length;

            yield return new WaitForSecondsRealtime(duration + 2); //Animation Length;

            ReloadScene();
        }

        private void ReloadScene() //TODO: Set this form a Game/Scene Manager Empty GameObject
        {
            //Reload Scene and reset components
            print("Scene Reloaded");
            SceneManager.LoadScene(0); 
            animator.SetTrigger("Death");
            isDead = false;
        }

        private void ReduceHealth(float damage)
        {
            currenthealthPoints = Mathf.Clamp(currenthealthPoints - damage, 0f, maxHealthPoints);
        }

        void OnMouseOverEnemy(Enemy enemy) {
            if (isDead == false)
            {
                if (Input.GetMouseButton(0) && IsTargetInRange(enemy))
                {
                    AttackTarget(enemy);
                }
                else if (Input.GetMouseButtonDown(1) && IsTargetInRange(enemy))
                {
                    AttemptSpecialAbility(0, enemy, characterStats.GetDamage());
                }
            }
        }

        private void AttemptSpecialAbility(int abilityIndex, IDamageable target, float amount)
        {
            if (playerEnergy.IsEnergyAvailable(abilities[abilityIndex].GetEnergyCost())) { 
                playerEnergy.ConsumeEnergy(abilities[abilityIndex].GetEnergyCost());
                if (abilityIndex == 0)
                {
                    float damageAmount = CalculateHitProbability(amount, target);
                    abilities[abilityIndex].Use(new AbilityUseParams(target, damageAmount));
                }
                else
                    abilities[abilityIndex].Use(new AbilityUseParams(target, amount));

            }
        }

        private void AttackTarget(Enemy target)
        {
            var enemyComponent = target;

            if (Time.time - lastHitTime > characterStats.GetActionSpeed())
            {

                animator.SetTrigger(ATTACK_TRIGGER);
                float hitValue = CalculateHitProbability(characterStats.GetDamage(), target);
                enemyComponent.TakeDamage(hitValue);
                lastHitTime = Time.time;
            }
        }
        public float CalculateHitProbability(float damage, IDamageable target)
        {
            int score = Random.Range(1, 101);
            Enemy enemy = target as Enemy;
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
            return distanceToTarget <= weaponInUse.MaxAttackRange;
        }
        public float HealthAsPercentage
        {
            get
            {
                return currenthealthPoints / (float)maxHealthPoints;
            }

        }

        void SetWeaponModifiersToPlayer()
        { 
            if (characterStats != null)
            {
                characterStats.SetActionSpeed(weaponInUse.ActionSpeedModifier);
                characterStats.SetDamage(weaponInUse.DamageModifier);
            }
        }

        public bool GetIsDead() {
            return this.isDead;
        }

    }
}