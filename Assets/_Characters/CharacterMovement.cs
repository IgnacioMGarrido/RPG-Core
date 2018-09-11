using System;
using UnityEngine;
using UnityEngine.AI;

using RPG.CameraUI;
namespace RPG.Characters
{
    [RequireComponent(typeof(ThirdPersonCharacter))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Player))]

    //TODO: Maybe join Player Script and the PlayerMovementScript in the same Script
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] float stoppingDistance = 1f;
        Player player;
        ThirdPersonCharacter character;

        Vector3 currentDestination, clickPoint;

        GameObject walkTarget;

        NavMeshAgent agent;

        private void Start()
        {
            player = GetComponent<Player>();
            RPGCursor rpgCursor = Camera.main.GetComponent<RPGCursor>();
            character = GetComponent<ThirdPersonCharacter>();
            walkTarget = new GameObject("WalkTarget");
            currentDestination = transform.position;

            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updatePosition = true;
            agent.stoppingDistance = stoppingDistance;

            rpgCursor.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
            rpgCursor.onMouseOverEnemy += OnMouseOverEnemy;

        }
        private void Update()
        {
            if (agent.remainingDistance > agent.stoppingDistance)
                character.Move(agent.desiredVelocity);
            else
                character.Move(Vector3.zero);
        }
        void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (player.GetIsDead() == false)
            {
                if (Input.GetMouseButton(0))
                {
                    agent.SetDestination(destination);

                }
            }

        }
        void OnMouseOverEnemy(Enemy enemy)
        {
            if (player.GetIsDead() == false)
            {
                if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
                {
                    agent.SetDestination(enemy.transform.position);
                }
            }
        }

        private void WalkToDestination()
        {
            var playerToClickPoint = currentDestination - transform.position;

            if (playerToClickPoint.magnitude >= 0)
            {
                character.Move(playerToClickPoint);
            }
            else
            {
                character.Move(Vector3.zero);
            }
        }

        private Vector3 ShortDestination(Vector3 destination, float shortening)
        {
            Vector3 reductionVector = (destination - transform.position).normalized * shortening;
            return destination - reductionVector;
        }

    }

}