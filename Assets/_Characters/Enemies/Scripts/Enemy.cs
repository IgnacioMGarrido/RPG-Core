using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using RPG.Core;

namespace RPG.Characters
{
    public class Enemy : MonoBehaviour
    {

        [SerializeField] float chaseRadius = 5.0f;
        [SerializeField] float attackRadius = 7.0f;
        [SerializeField] float stopChasingRadius = 20.0f;
        [SerializeField] GameObject projectileToUse;
        [SerializeField] Vector3 AimOffset = new Vector3(0f, 1f, 0f);

        [SerializeField] GameObject projectileSocket;

        Animator animator;

        bool isAttacking = false;

        Transform originalTransform;
        GameObject player = null;

        CharacterStats characterStats;
        Character enemyCharacter;
        SpecialAbilities enemyAbilities;

        GameObject spawnPosition;

        float lastHitTime = 0f;
        private void Start()
        {

            player = GameObject.FindGameObjectWithTag("Player");
            characterStats = GetComponent<CharacterStats>();
            enemyCharacter = GetComponent<Character>();
            enemyAbilities = GetComponent<SpecialAbilities>();

            spawnPosition = new GameObject("SpawnPosition");
            spawnPosition.transform.position = transform.position;
            spawnPosition.transform.parent = GameObject.Find("SpawnPositions").transform;
        }

        private void Update()
        {

            float distanceToPlayer = Mathf.Abs(Vector3.Distance(player.transform.position, transform.position));
            float spawnDistanceToPlayer = Mathf.Abs(Vector3.Distance(player.transform.position, spawnPosition.transform.position));

            if (distanceToPlayer <= attackRadius && !isAttacking)
            {
                isAttacking = true;
                InvokeRepeating("AttackTarget", 0f, characterStats.GetActionSpeed()); //TODO: Switch to coroutines
            }
            if (distanceToPlayer > attackRadius)
            {
                isAttacking = false;
                CancelInvoke();
            }

            if (distanceToPlayer <= chaseRadius)
            {
                enemyCharacter.SetDestination(player.transform.position);
            }
            else if (spawnDistanceToPlayer >= stopChasingRadius)
            {
                enemyCharacter.SetDestination(spawnPosition.transform.position);
            }
        }

        void AttackTarget() {
            enemyCharacter.AttackTarget(player.GetComponent<HealthSystem>());
        }
        private void FireProjectile()
        {
            GameObject newProjectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.LookRotation(player.transform.position.normalized));
            Vector3 unitVectorToPlayer = (player.transform.position + AimOffset - projectileSocket.transform.position).normalized;
            var projectileComponent = newProjectile.GetComponent<Projectile>();
            projectileComponent.SetShooter(this.gameObject);
            //projectileComponent.SetDamage(damagePerShot);

            float projectileSpeed = projectileComponent.getDefaultLaunchSpeed();
            newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;
        }

        void OnDrawGizmos()
        {

            Gizmos.color = new Color(0f, 0f, 255f, .5f);
            Gizmos.DrawWireSphere(transform.position, chaseRadius);

            Gizmos.color = new Color(255f, 0f, 0f, .5f);
            Gizmos.DrawWireSphere(transform.position, attackRadius);
        }

        public void TakeHeal(float heal)
        {
            throw new System.NotImplementedException();
        }
    }
}