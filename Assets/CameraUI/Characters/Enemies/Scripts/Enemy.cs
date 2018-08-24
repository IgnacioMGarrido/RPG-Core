using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour, IDamageable
{

    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] float damagePerShot = 15.0f;
    [SerializeField] float secondsBetweenShots = 0.5f;

    [SerializeField] float chaseRadius = 5.0f;
    [SerializeField] float attackRadius = 7.0f;
    [SerializeField] float stopChasingRadius = 20.0f;
    [SerializeField] GameObject projectileToUse;
    [SerializeField] Vector3 AimOffset = new Vector3(0f,1f,0f);
    int projectileCont = 0;

    [SerializeField] GameObject projectileSocket;


    bool isAttacking = false;

    AICharacterControl aiCharacterController = null;
    CharacterStats characterStats;
    Transform originalTransform;
    GameObject player = null;
    float currenthealthPoints = 100f;

    GameObject spawnPosition;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        aiCharacterController = GetComponent<AICharacterControl>();
        characterStats = GetComponent<CharacterStats>();
        if (characterStats) {
            maxHealthPoints = characterStats.Health;
            damagePerShot = characterStats.Damage;
            secondsBetweenShots = characterStats.ActionSpeed;
        }
        currenthealthPoints = maxHealthPoints;
        spawnPosition = new GameObject("SpawnPosition");
        spawnPosition.transform.position = transform.position;
        spawnPosition.transform.parent = GameObject.Find("SpawnPositions").transform; // TODO: Change this so is the enemy empty gameObject who creates this object

    }

    private void Update()
    {
        float distanceToPlayer = Mathf.Abs(Vector3.Distance(player.transform.position, transform.position));
        float spawnDistanceToPlayer = Mathf.Abs(Vector3.Distance(player.transform.position, spawnPosition.transform.position));

        if (distanceToPlayer <= attackRadius && !isAttacking)
        {
            isAttacking = true;
            InvokeRepeating("SpawnProjectile", 0f, secondsBetweenShots); //TODO: Switch to coroutines
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
    private void SpawnProjectile()
    {

        GameObject newProjectile;
        
        newProjectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.LookRotation(player.transform.position.normalized));        

        Vector3 unitVectorToPlayer = (player.transform.position + AimOffset - projectileSocket.transform.position).normalized;
        var projectileComponent = newProjectile.GetComponent<Projectile>();
        projectileComponent.SetDamage(damagePerShot);
        float projectileSpeed = projectileComponent.projectileSpeed;

        

        newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;
    }
    public void TakeDamage(float damage)
    {
        currenthealthPoints = Mathf.Clamp(currenthealthPoints - damage, 0f, maxHealthPoints);
        if (currenthealthPoints <= 0) {
            Destroy(this.gameObject);
        }
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
        

        /*   if (spawnPosition != null)
           {
               Gizmos.color = new Color(255f, 255f, 0f, .5f);
               Gizmos.DrawWireSphere(spawnPosition.transform.position, stopChasingRadius);
           }
       */
    }
}
