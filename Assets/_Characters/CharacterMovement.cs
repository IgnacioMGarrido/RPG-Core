using System;
using UnityEngine;
using UnityEngine.AI;

using RPG.CameraUI;
namespace RPG.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Player))]

    //TODO: Maybe join Player Script and the PlayerMovementScript in the same Script
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] float stoppingDistance = 1f;
        [SerializeField] float movingTurnSpeed = 360;
        [SerializeField] float stationaryTurnSpeed = 180;
        [SerializeField] float moveThreshold = 1f;
        [SerializeField] float moveSpeedMultiplier = 1.2f;
        [SerializeField] float animatiorSpeedMultiplier = 1f;

        Player player;

        Vector3 currentDestination, clickPoint;

        NavMeshAgent agent;
        Rigidbody myRigidbody;
        Animator animator;

        float turnAmount;
        float forwardAmount;

        private void Start()
        {
            player = GetComponent<Player>();

            myRigidbody = GetComponent<Rigidbody>();
            myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            animator = GetComponent<Animator>();
            animator.applyRootMotion = true; //TODO: Consider if needed.

            RPGCursor rpgCursor = Camera.main.GetComponent<RPGCursor>();
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
                Move(agent.desiredVelocity);
            else
                Move(Vector3.zero);
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

        public void Kill()
        {
            //TO allow Death Signaling.
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
                Move(playerToClickPoint);
            }
            else
            {
                Move(Vector3.zero);
            }
        }

        private Vector3 ShortDestination(Vector3 destination, float shortening)
        {
            Vector3 reductionVector = (destination - transform.position).normalized * shortening;
            return destination - reductionVector;
        }

        public void Move(Vector3 movement)
        {
            SetForwardAndTurn(movement);
            ApplyExtraTurnRotation();
            UpdateAnimator();
        }

        private void SetForwardAndTurn(Vector3 movement)
        {
            if (movement.magnitude > moveThreshold)
            {
                movement.Normalize();
            }
            var localMove = transform.InverseTransformDirection(movement);
            turnAmount = Mathf.Atan2(localMove.x, localMove.z);
            forwardAmount = localMove.z;
        }

        private void UpdateAnimator()
        {
            animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
            animator.speed = animatiorSpeedMultiplier;
        }
        private void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (Time.deltaTime > 0)
            {
                Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;
                // we preserve the existing y part of the current velocity.
                velocity.y = myRigidbody.velocity.y;
                myRigidbody.velocity = velocity;
            }
        }
    }

}