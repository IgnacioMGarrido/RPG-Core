using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{

    [SerializeField] float walkMoveStopRadius = .2f;

    ThirdPersonCharacter m_Character;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster cameraRaycaster;
    Vector3 currentClickTarget;


    bool m_Jump = false;
    bool isInDirectMode = false; //TODO: Consider make it Static later


    private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        m_Character = GetComponent<ThirdPersonCharacter>();
        currentClickTarget = transform.position;
    }
    //TODO: Fix conflict between click movement and WASD movement
    private void Update()
    {
        /*  if (!m_Jump)
          {
              m_Jump = Input.GetButtonDown("Jump");
          }*/
        //TODO: Let the user map later or add to menu
        if (Input.GetKeyDown(KeyCode.G)) 
        {
            isInDirectMode = !isInDirectMode;
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
            print("Cursor raycast hit" + cameraRaycaster.layerHit);//hit.collider.gameObject.name.ToString());
            switch (cameraRaycaster.layerHit)
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
            m_Character.Move(playerToClickPoint, crouch, false);
        }
        else
        {
            m_Character.Move(Vector3.zero, crouch, false);
        }
    }

    private void ProcessDirectMovement(bool crouch)
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 m_CamForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 m_Move = v * m_CamForward + h * Camera.main.transform.right;

        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
        m_Character.Move(m_Move, crouch, m_Jump);

    }
}

