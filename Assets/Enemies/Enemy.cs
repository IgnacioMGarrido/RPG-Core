using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour {

    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] float chaseRadius = 5.0f;

    AICharacterControl aiCharacterController = null;
    Transform originalTransform;
    GameObject player = null;
    float currenthealthPoints = 100f;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        aiCharacterController = GetComponent<AICharacterControl>();

    }

    private void Update()
    {
        float distanceToPlayer = Mathf.Abs(Vector3.Distance(player.transform.position, transform.position));
        if (distanceToPlayer <= chaseRadius)
        {
            aiCharacterController.SetTarget(player.transform);
        }
        else
        {
            aiCharacterController.SetTarget(originalTransform);
        }
    }
    public float healthAsPercentage
    {
        get
        {
            return currenthealthPoints / (float)maxHealthPoints;
        }

    }
}
