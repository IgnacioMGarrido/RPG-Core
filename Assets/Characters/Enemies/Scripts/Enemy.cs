using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using RPG.Weapons;
using RPG.Core;

namespace RPG.Characters
{
    public class Enemy : MonoBehaviour, IDamageable
    {

        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float damagePerShot = 15.0f;
        [SerializeField] float actionSpeed = 0.5f;

        [SerializeField] float chaseRadius = 5.0f;
        [SerializeField] float attackRadius = 7.0f;
        [SerializeField] float stopChasingRadius = 20.0f;
        [SerializeField] GameObject projectileToUse;
        [SerializeField] Vector3 AimOffset = new Vector3(0f, 1f, 0f);

        [SerializeField] GameObject projectileSocket;

        [Header("Weapon")]
        [SerializeField] Weapon weaponInUse = null;

        [SerializeField] AnimatorOverrideController animatorOverrideController;
        Animator animator;

        bool isAttacking = false;

        AICharacterControl aiCharacterController = null;
        CharacterStats characterStats;
        Transform originalTransform;
        GameObject player = null;
        float currenthealthPoints = 100f;

        GameObject spawnPosition;

        float lastHitTime = 0f;
        private void Start()
        {

            player = GameObject.FindGameObjectWithTag("Player");
            aiCharacterController = GetComponent<AICharacterControl>();
            characterStats = GetComponent<CharacterStats>();
            if (characterStats)
            {
                maxHealthPoints = characterStats.GetHealth();
                damagePerShot = characterStats.GetDamage();
                actionSpeed = characterStats.GetActionSpeed();
            }
            currenthealthPoints = maxHealthPoints;
            spawnPosition = new GameObject("SpawnPosition");
            spawnPosition.transform.position = transform.position;
            spawnPosition.transform.parent = GameObject.Find("SpawnPositions").transform;

            PutWeaponInHand();
            SetupRuntimeAnimator();
        }
        void PutWeaponInHand()
        {

            GameObject weapon = Instantiate(weaponInUse.WeaponPrefab); //, weaponSlot.position, weaponSlot.rotation) as GameObject;
            GameObject dominantHandSocket = RequestDominantHand();
            weapon.transform.SetParent(dominantHandSocket.transform);
            weapon.transform.localPosition = weaponInUse.Grip.localPosition;
            weapon.transform.localRotation = weaponInUse.Grip.localRotation;

            //SetWeaponModifiersToEnemy(); //TODO: Set Enemy Weapon modifiers.
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
        private void Update()
        {
            if (player.GetComponent<Player>().HealthAsPercentage <= Mathf.Epsilon) { //Stop Coroutines
                StopAllCoroutines();
                Destroy(this);
            }

            float distanceToPlayer = Mathf.Abs(Vector3.Distance(player.transform.position, transform.position));
            float spawnDistanceToPlayer = Mathf.Abs(Vector3.Distance(player.transform.position, spawnPosition.transform.position));

            if (distanceToPlayer <= attackRadius && !isAttacking)
            {
                isAttacking = true;
                InvokeRepeating("AttackTarget", 0f, actionSpeed); //TODO: Switch to coroutines
            }
            if (distanceToPlayer > attackRadius)
            {
                isAttacking = false;
                CancelInvoke();
            }

            if (distanceToPlayer <= chaseRadius)
            {
                aiCharacterController.SetTarget(player.transform);
            }
            else if (spawnDistanceToPlayer >= stopChasingRadius)
            {
                aiCharacterController.SetTarget(spawnPosition.transform);
            }
        }

        private void SetupRuntimeAnimator()
        {

            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController["DEFAULT ATTACK"] = weaponInUse.GetAttackAnimClip(); //remove const
        }

        //TODO: Start separating Character firing logic
        private void FireProjectile()
        {
            GameObject newProjectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.LookRotation(player.transform.position.normalized));
            Vector3 unitVectorToPlayer = (player.transform.position + AimOffset - projectileSocket.transform.position).normalized;
            var projectileComponent = newProjectile.GetComponent<Projectile>();
            projectileComponent.SetShooter(this.gameObject);
            projectileComponent.SetDamage(damagePerShot);

            float projectileSpeed = projectileComponent.getDefaultLaunchSpeed();
            newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;
        }

        private void AttackTarget()
        {
            var playerComponent = player.GetComponent<Player>();

            if (Time.time - lastHitTime > characterStats.GetActionSpeed())
            {
                //animator.SetTrigger("Attack");
                animator.SetTrigger("Attack");
                FireProjectile();
                float hitValue = CalculateHitProbability(characterStats.GetDamage(),player.GetComponent<Player>());
                playerComponent.TakeDamage(hitValue);
                lastHitTime = Time.time;
            }
        }

        public void TakeDamage(float damage)
        {
            currenthealthPoints = Mathf.Clamp(currenthealthPoints - damage, 0f, maxHealthPoints);
            if (currenthealthPoints <= 0)
            {
                Destroy(this.gameObject);
            }
        }

        public float CalculateHitProbability(float damage, IDamageable target)
        {
            int score = Random.Range(1, 101);
            float damageDealerNewAccuracy = GetComponent<CharacterStats>().GetAccuracy() - player.GetComponent<CharacterStats>().GetDeflection();
            float attackRoll = score + damageDealerNewAccuracy;
            //print("------------------------------------------------------------------------------");
            //print("Attack Roll: " + score + " + " + damageDealerNewAccuracy + " = " + attackRoll);
            if (attackRoll > 25 && attackRoll <= 50)
            {
                damage = damage / 2;
            //    print("This hit was a GRAZE. Damage = " + damage);
            }
            else if (attackRoll > 100)
            {
                damage = damage * 1.25f;
            //    print("This hit was a CRIT HIT. Damage = " + damage);

            }
            else
            {
            //    print("This hit was a NORMAL HIT. Damage = " + damage);
            }

            return damage;
        }

        public float healthAsPercentage
        {
            get
            {
                return currenthealthPoints / (float)maxHealthPoints;
            }

        }

        void OnDrawGizmos()
        {

            Gizmos.color = new Color(0f, 0f, 255f, .5f);
            Gizmos.DrawWireSphere(transform.position, chaseRadius);

            Gizmos.color = new Color(255f, 0f, 0f, .5f);
            Gizmos.DrawWireSphere(transform.position, attackRadius);
        }

    }
}