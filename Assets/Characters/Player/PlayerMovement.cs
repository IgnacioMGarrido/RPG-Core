using System;
using UnityEngine;
using UnityEngine.AI;

using RPG.CameraUI;
namespace RPG.Characters
{
    [RequireComponent(typeof(ThirdPersonCharacter))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(AICharacterControl))]
    [RequireComponent(typeof(Player))]

    //TODO: Maybe join Player Script and the PlayerMovementScript in the same Script
    public class PlayerMovement : MonoBehaviour
    {
        Player player = null;
        ThirdPersonCharacter thirdPersonCharacter = null;   // A reference to the ThirdPersonCharacter on the object
        AICharacterControl aiCharacterControl = null;

        RPGCursor cameraRaycaster;
        Vector3 currentDestination, clickPoint;


        GameObject walkTarget = null;
        bool isInDirectMode = false;


        private void Start()
        {
            player = GetComponent<Player>();
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
            if (player.GetIsDead() == false)
            {
                if (Input.GetMouseButton(0))
                {
                    walkTarget.transform.position = destination;
                    aiCharacterControl.SetTarget(walkTarget.transform);
                }
            }
            else {
                walkTarget.transform.position = this.transform.position;
                aiCharacterControl.SetTarget(walkTarget.transform);
            }
        }
        void OnMouseOverEnemy(Enemy enemy)
        {
            if (player.GetIsDead() == false)
            {
                if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
                {
                    aiCharacterControl.SetTarget(enemy.transform);
                }
            }
            else {
                aiCharacterControl.SetTarget(this.transform);
            }
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