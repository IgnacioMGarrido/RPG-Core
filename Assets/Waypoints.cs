using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDrawGizmos()
    {
        Vector3 firstPosition = transform.GetChild(0).position;
        Vector3 previousPosition = firstPosition;
        foreach (Transform waypoint in transform)
        {
            Gizmos.DrawSphere(waypoint.position, .1f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }

        Gizmos.DrawLine(previousPosition, firstPosition);


    }
}
