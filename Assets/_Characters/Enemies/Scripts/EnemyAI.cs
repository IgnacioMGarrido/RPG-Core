using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using RPG.Core;
using System;

namespace RPG.Characters
{
    public class EnemyAI : MonoBehaviour
    {

        private enum State { Patrol, Attack, Chase, Idle, Dead }
        private State currentState = State.Idle;
        [SerializeField] Waypoints patrolPath;
        [SerializeField] float chaseRadius = 5.0f;
        [SerializeField] float attackRadius = 7.0f;
        [SerializeField] float waypointTolerance = 2f;
        //[SerializeField] float stopChasingRadius = 20.0f;
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
        WeaponSystem weaponSystem;

        GameObject spawnPosition;
        float distanceToPlayer;
        int nextWaypointIndex = 0;
        private void Start()
        {

            player = GameObject.FindGameObjectWithTag("Player");
            characterStats = GetComponent<CharacterStats>();
            weaponSystem = GetComponent<WeaponSystem>();
            enemyCharacter = GetComponent<Character>();
            enemyAbilities = GetComponent<SpecialAbilities>();

            spawnPosition = new GameObject("SpawnPosition");
            spawnPosition.transform.position = transform.position;
            spawnPosition.transform.parent = GameObject.Find("SpawnPositions").transform;
        }

        private void Update()
        {
            //TODO: Reconsider making this delegates.
            if (GetComponent<HealthSystem>().GetIsAlive() == true)
            {
                distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

                if (distanceToPlayer > chaseRadius && currentState != State.Patrol)
                {
                    StopAllCoroutines();
                    StartCoroutine(Patrol());
                }
                if (distanceToPlayer <= chaseRadius && currentState != State.Chase)
                {
                    StopAllCoroutines();
                    StartCoroutine(ChasePlayer());
                }
                if (distanceToPlayer <= attackRadius && currentState != State.Attack)
                {
                    StopAllCoroutines();
                    StartCoroutine(AttackRepeatedly());

                }
            }
            if (GetComponent<HealthSystem>().GetIsAlive() == false && currentState != State.Dead)
            {
                currentState = State.Dead;
                StopAllCoroutines();
                
            }


        }

        private IEnumerator AttackRepeatedly()
        {
            currentState = State.Attack;
            //weaponSystem.AttackTarget(player.GetComponent<HealthSystem>());
            yield return new WaitForEndOfFrame();
        }

        private IEnumerator Patrol()
        {
            currentState = State.Patrol;

            while (true)
            {
                Vector3 nextWaypointPosition = patrolPath.transform.GetChild(nextWaypointIndex).position;
                //Set destination
                enemyCharacter.SetDestination(nextWaypointPosition);
                //Cycle waypoints
                SwitchToNextWaypoint(nextWaypointPosition);

                yield return new WaitForSeconds(.5f);
            }
        }

        private void SwitchToNextWaypoint(Vector3 nextWaypointPosition)
        {
            if (Vector3.Distance(transform.position, nextWaypointPosition) <= waypointTolerance)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
            }
        }

        private IEnumerator ChasePlayer()
        {
            currentState = State.Chase;
            while (distanceToPlayer >= attackRadius)
            {
                enemyCharacter.SetDestination(player.transform.position);
                yield return new WaitForEndOfFrame();
            }
        }

        void AttackTarget() {
            weaponSystem.AttackTarget(player.GetComponent<HealthSystem>());
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

    }
}