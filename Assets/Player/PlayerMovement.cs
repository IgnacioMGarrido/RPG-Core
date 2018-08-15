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


    bool isInDirectMode = false; 


    private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        currentDestination = transform.position;

        cameraRaycaster.notifyMouseClickObservers += ProcessIndirectMouseMovement;

    }
    private void Update()
    {
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

          if (isInDirectMode == true)
              ProcessDirectMovement();
         // else
              //ProcessIndirectMouseMovement();
      }

      private void ProcessIndirectMouseMovement(RaycastHit raycastHit, int layerHit)
      {
          if (Input.GetMouseButton(0))
          {
              clickPoint = raycastHit.point;
              switch (layerHit)
              {
                  case 9:
                      currentDestination = clickPoint;
                      currentDestination = ShortDestination(clickPoint, walkMoveStopRadius);
                      break;
                  case 10:
                      currentDestination = ShortDestination(clickPoint, attackMoveStopRadius);
                      Debug.Log("Not moving to enemy");
                      break;
                  default:
                      Debug.Log("Unexpected Layer Found.");
                      return;
              }
          }
          WalkToDestination();
      }
  
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

