using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    // Use this for initialization
    [SerializeField] float maxHealthPoints = 100f;
    float currenthealthPoints = 100f;

    public float healthAsPercentage
    {
        get
        {
            return currenthealthPoints / (float) maxHealthPoints;
        }

    }

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
