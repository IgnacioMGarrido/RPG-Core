using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{

    // Use this for initialization
    [SerializeField] int enemyLayer = 10;
    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] float damagePerHit = 10f;
    [SerializeField] float minTimeBetweenHits = 0.5f;
    [SerializeField] float maxAttackRange = 1.5f;



    GameObject currentTarget = null;
    CameraRaycaster cameraRaycaster;

    float currenthealthPoints = 100f;
    float lastHitTime = 0f;
    void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
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

            if (Time.time - lastHitTime > minTimeBetweenHits)
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


}
