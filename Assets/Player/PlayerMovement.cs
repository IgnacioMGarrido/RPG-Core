using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{

    [SerializeField] float walkMoveStopRadius = .2f;

    ThirdPersonCharacter thirdPersonCharacter;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster cameraRaycaster;
    Vector3 currentClickTarget;


    bool bJump = false;
    bool isInDirectMode = false; 


    private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        currentClickTarget = transform.position;
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
            currentClickTarget = transform.position;
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
            print("Cursor raycast hit" + cameraRaycaster.currentLayerHit);//hit.collider.gameObject.name.ToString());
            switch (cameraRaycaster.currentLayerHit)
            {
                case Layer.Walkable:
                    currentClickTarget = cameraRaycaster.hit.point;
                    break;
                case Layer.Enemy:
                    Debug.Log("Not moving to enemy");
                    break;
                default:
                    Debug.Log("Unexpected Layer Found.");
                    return;
            }
        }
        var playerToClickPoint = currentClickTarget - transform.position;

        if (playerToClickPoint.magnitude >= walkMoveStopRadius)
        {
            thirdPersonCharacter.Move(playerToClickPoint, crouch, false);
        }
        else
        {
            thirdPersonCharacter.Move(Vector3.zero, crouch, false);
        }
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
}

