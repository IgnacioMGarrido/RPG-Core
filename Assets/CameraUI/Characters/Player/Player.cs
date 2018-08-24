using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{

    // Use this for initialization
    [SerializeField] int enemyLayer = 10;
    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] float damagePerHit = 10f;
    [SerializeField] float actionSpeed = 0.5f;
    [SerializeField] float maxAttackRange = 1.5f;

    [SerializeField] Transform weaponSlot = null;
    [SerializeField] Weapon weaponInUse = null;

    GameObject currentTarget = null;
    CameraRaycaster cameraRaycaster;
    CharacterStats characterStats;

    [SerializeField] float currenthealthPoints = 100f;
    float lastHitTime = 0f;
    void Start()
    {
        characterStats = GetComponent<CharacterStats>();
        if (characterStats != null) {
            maxHealthPoints = characterStats.Health;
            damagePerHit = characterStats.Damage;
            actionSpeed = characterStats.ActionSpeed;
        }
        if (weaponSlot == null) { Debug.LogError("Player has not assigned a weapon Slot. Assign it on inspector"); }
        InstantiateWeapon();

        currenthealthPoints = maxHealthPoints;
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
    }
    void InstantiateWeapon() {

        GameObject currentWeapon = Instantiate(weaponInUse.WeaponPrefab, weaponSlot.position, weaponSlot.rotation) as GameObject;
        currentWeapon.transform.SetParent(weaponSlot);
        SetWeaponModifiersToPlayer();
    }
    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseClick(RaycastHit raycastHit, int layerHit)
    {
        //TODO: check dependencies.
        if (layerHit == enemyLayer) {
            var enemy = raycastHit.collider.gameObject;
            
            //Check enemy is in range.
            if((enemy.transform.position - transform.position).magnitude > maxAttackRange){
                return;
            }

            currentTarget = enemy;
            var enemyComponent = currentTarget.GetComponent<Enemy>();

            if (Time.time - lastHitTime > actionSpeed)
            {
                enemyComponent.TakeDamage(damagePerHit);
                lastHitTime = Time.time;
            }
        }
    }

    public float healthAsPercentage
    {
        get
        {
            return currenthealthPoints / (float) maxHealthPoints;
        }

    }

    public void TakeDamage(float damage)
    {
        currenthealthPoints = Mathf.Clamp(currenthealthPoints - damage, 0f, maxHealthPoints);
    }

    void SetWeaponModifiersToPlayer() { //TODO we may want to modify the player stats instead?
        damagePerHit = damagePerHit + damagePerHit * weaponInUse.AttackPercentageModifier;
        actionSpeed = actionSpeed + actionSpeed * weaponInUse.SpeedPenaltyPercentageModifier;
    }


    
}
