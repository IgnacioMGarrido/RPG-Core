using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.AI;
[RequireComponent(typeof (ThirdPersonCharacter))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AICharacterControl))]


public class PlayerMovement : MonoBehaviour
{

    const int enemyLayerNumber = 10;
    const int walkableLayerNumber = 9;

    //  [SerializeField] float walkMoveStopRadius = .2f;
    //  [SerializeField] float attackMoveStopRadius = 5f;


    ThirdPersonCharacter thirdPersonCharacter = null;   // A reference to the ThirdPersonCharacter on the object
    AICharacterControl aiCharacterControl = null;

    CameraRaycaster cameraRaycaster;
    Vector3 currentDestination, clickPoint;


    GameObject walkTarget = null;
    bool isInDirectMode = false; 


    private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        aiCharacterControl = GetComponent<AICharacterControl>();
        walkTarget = new GameObject("WalkTarget");
        currentDestination = transform.position;

        cameraRaycaster.notifyMouseClickObservers += ProcessMouseClick;

    }

    void ProcessMouseClick(RaycastHit raycastHit, int layerHit) {

        switch (layerHit)
        {
            case walkableLayerNumber:
                walkTarget.transform.position = raycastHit.point;
                aiCharacterControl.SetTarget(walkTarget.transform);
                break;
            case enemyLayerNumber:
                GameObject enemy = raycastHit.collider.gameObject;
                aiCharacterControl.SetTarget(enemy.transform);
                break;
            default:
                Debug.Log("Unexpected Layer Found.");
                return;
        }
        
        //WalkToDestination();
    }

  
    //TODO: Make this called again.
    private void ProcessDirectMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 camForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 move = v * camForward + h * Camera.main.transform.right;

        if (Input.GetKey(KeyCode.LeftShift)) move *= 0.5f;
        thirdPersonCharacter.Move(move, false, false);
    }
    private void WalkToDestination()
    {
        var playerToClickPoint = currentDestination - transform.position;

        if (playerToClickPoint.magnitude >= 0)
        {
            thirdPersonCharacter.Move(playerToClickPoint, false, false);
        }
        else
        {
            thirdPersonCharacter.Move(Vector3.zero, false, false);
        }
    }

    private Vector3 ShortDestination(Vector3 destination, float shortening)
    {
        Vector3 reductionVector = (destination - transform.position).normalized * shortening;
        return destination - reductionVector;
    }

}

