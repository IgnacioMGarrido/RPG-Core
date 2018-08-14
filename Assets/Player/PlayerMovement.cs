using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{

    [SerializeField] float walkMoveStopRadius = .2f;
    [SerializeField] float attackMoveStopRadius = 5f;


    ThirdPersonCharacter thirdPersonCharacter;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster cameraRaycaster;
    Vector3 currentDestination;
    Vector3 clickPoint;



    bool bJump = false;
    bool isInDirectMode = false; 


    private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        currentDestination = transform.position;
    }
    private void Update()
    {
        /*  if (!jump)
          {
              jump = Input.GetButtonDown("Jump");
          }*/
        //TODO: Let the user map later or add to menu
        if (Input.GetKeyDown(KeyCode.G)) 
        {
            isInDirectMode = !isInDirectMode;
            currentDestination = transform.position;
        }
    }
    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        bool crouch = Input.GetKey(KeyCode.C);

        if (isInDirectMode == true)
            ProcessDirectMovement(crouch);
        else
            ProcessIndirectMouseMovement(crouch);
    }

    private void ProcessIndirectMouseMovement(bool crouch)
    {
        if (Input.GetMouseButton(0))
        {
            clickPoint = cameraRaycaster.hit.point;
            switch (cameraRaycaster.currentLayerHit)
            {
                case Layer.Walkable:
                    currentDestination = clickPoint;
                    currentDestination = ShortDestination(clickPoint, walkMoveStopRadius);
                    break;
                case Layer.Enemy:
                    currentDestination = ShortDestination(clickPoint, attackMoveStopRadius);
                    Debug.Log("Not moving to enemy");
                    break;
                default:
                    Debug.Log("Unexpected Layer Found.");
                    return;
            }
        }
        WalkToDestination(crouch);
    }

    private void WalkToDestination(bool crouch)
    {
        var playerToClickPoint = currentDestination - transform.position;

        if (playerToClickPoint.magnitude >= 0)
        {
            thirdPersonCharacter.Move(playerToClickPoint, crouch, false);
        }
        else
        {
            thirdPersonCharacter.Move(Vector3.zero, crouch, false);
        }
    }

    private Vector3 ShortDestination(Vector3 destination, float shortening)
    {
        Vector3 reductionVector = (destination - transform.position).normalized * shortening;
        return destination - reductionVector;
    }

    private void ProcessDirectMovement(bool crouch)
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 camForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 move = v * camForward + h * Camera.main.transform.right;

        if (Input.GetKey(KeyCode.LeftShift)) move *= 0.5f;
        thirdPersonCharacter.Move(move, crouch, bJump);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        Gizmos.DrawLine(transform.position, currentDestination);
        Gizmos.DrawSphere(currentDestination, 0.1f);
        Gizmos.DrawLine(currentDestination, clickPoint);
        Gizmos.DrawSphere(clickPoint, 0.05f);

        //Draw attack spehere
        Gizmos.color = new Color(255f, 0f,0f, .5f);
        //Gizmos.DrawSphere(transform.position, attackMoveStopRadius);


    }
}

