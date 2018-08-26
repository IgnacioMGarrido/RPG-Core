using System;
using UnityEngine;
using UnityEngine.AI;

using RPG.CameraUI;
namespace RPG.Characters
{
    [RequireComponent(typeof(ThirdPersonCharacter))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(AICharacterControl))]


    public class PlayerMovement : MonoBehaviour
    {


        //  [SerializeField] float walkMoveStopRadius = .2f;
        //  [SerializeField] float attackMoveStopRadius = 5f;


        ThirdPersonCharacter thirdPersonCharacter = null;   // A reference to the ThirdPersonCharacter on the object
        AICharacterControl aiCharacterControl = null;

        RPGCursor cameraRaycaster;
        Vector3 currentDestination, clickPoint;


        GameObject walkTarget = null;
        bool isInDirectMode = false;


        private void Start()
        {
            cameraRaycaster = Camera.main.GetComponent<RPGCursor>();
            thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
            aiCharacterControl = GetComponent<AICharacterControl>();
            walkTarget = new GameObject("WalkTarget");
            currentDestination = transform.position;

            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;

        }
        void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                walkTarget.transform.position = destination;
                aiCharacterControl.SetTarget(walkTarget.transform);
            }
        }
        void OnMouseOverEnemy(Enemy enemy)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
            {
                aiCharacterControl.SetTarget(enemy.transform);
            }
        }


        //TODO: Make this called again. (Don't know yet if I want to implement gamepad movement)
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

}