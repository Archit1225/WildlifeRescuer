using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Stag : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private Animator animator;
    public Transform player;

    public LayerMask groundLayer, playerLayer, obstacleLayer;

    //Animal Properties
    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float runSpeed = 5f;

    //Patrolling
    private bool walkPointSet;
    private Vector3 walkPoint;
    public float walkPointRange = 20f;
    public float fleeDistance = 20f;

    //Detection
    public float raycastLength = 8f;
    public Transform raycastSource;
    public float visualConeAngle = 60f;
    public float detectionRange = 10f;
    private bool moveTowardsTrap = false;
    private Trap targetTrap;

    //States
    public enum AnimalState { Injured, Eating, Idle, Roaming, Fleeing, Attacking }
    private AnimalState currentState;
    private bool canWalk;

    // Passive Timers
    private float passiveTimer;
    public float timeSpentIdlingOrEating = 3f;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentState = AnimalState.Idle;

        navAgent.speed = walkSpeed;
        canWalk = true;
        walkPointSet = false;
    }

    private void Update()
    {
        if (player == null || !canWalk) return;

        CheckForThreats();

        switch (currentState)
        {
            case AnimalState.Idle:
            case AnimalState.Eating:
                HandlePassiveStates();
                break;
            case AnimalState.Roaming:
                Roam();
                break;
            case AnimalState.Fleeing:
                HandleFleeingState();
                break;
            case AnimalState.Injured:
                moveTowardsTrap = false;
                targetTrap = null;
                break;
        }
    }

    private void HandlePassiveStates()
    {
        passiveTimer -= Time.deltaTime;

        if (passiveTimer <= 0)
        {
            DecideNextPassiveAction();
        }
    }

    private void DecideNextPassiveAction()
    {
        float random = Random.Range(0f, 1f);

        if (random <= 0.3f)
        {
            ChangeState(AnimalState.Roaming);
        }
        else if (random <= 0.6f)
        {
            ChangeState(AnimalState.Eating);
            passiveTimer = timeSpentIdlingOrEating;
        }
        else if (random <= 0.9f)
        {
            ChangeState(AnimalState.Idle);
            passiveTimer = timeSpentIdlingOrEating;
        }
        else
        {
            //Move Towards Trap
            if (TrapManager.Instance != null)
            {
                targetTrap = TrapManager.Instance.GetRandomActiveTrapInRadius(transform.position, 25f);
            }

            if (targetTrap != null)
            {
                ChangeState(AnimalState.Roaming);
                moveTowardsTrap = true;
                walkPoint = targetTrap.transform.position;
                navAgent.SetDestination(walkPoint);
                walkPointSet = true;
            }
            else
            {
                ChangeState(AnimalState.Roaming);
                moveTowardsTrap = false;
            }
        }
    }

    private void CheckForThreats()
    {
        if (currentState == AnimalState.Injured) return;
        Debug.Log("Is this checking");
        Vector3 directionToPlayer = (player.position - raycastSource.position).normalized;

        //Debug.Log($"Distance from player - {Vector3.Distance(player.position, transform.position)}");
        Debug.Log($"Position of player - {player.position}");
        if (Vector3.Distance(player.position, transform.position) <= detectionRange)
        {
            float angleToPlayer = Vector3.Angle(raycastSource.forward, directionToPlayer);
            Debug.Log("Inside a sphere");
;            if (angleToPlayer <= visualConeAngle / 2f)
            {
                if (!Physics.Raycast(raycastSource.position, directionToPlayer, raycastLength, obstacleLayer))
                {
                    Debug.Log("Raycast done");
                    if (currentState != AnimalState.Fleeing)
                    {
                        ChangeState(AnimalState.Fleeing);
                        walkPointSet = false;
                        moveTowardsTrap = false;
                        targetTrap = null;
                    }
                }
            }
        }
    }

    private void HandleFleeingState()
    {
        navAgent.speed = runSpeed;

        if (!walkPointSet)
        {
            Vector3 directionToPlayer = (transform.position - player.position).normalized;
            Vector3 runPoint = transform.position + (directionToPlayer * fleeDistance);

            if (NavMesh.SamplePosition(runPoint, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                navAgent.SetDestination(hit.position);
                walkPointSet = true;
            }
        }
        else
        {
            if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance + 0.5f)
            {
                walkPointSet = false;

                if (Vector3.Distance(player.position, transform.position) > detectionRange)
                {
                    ChangeState(AnimalState.Idle);
                    passiveTimer = timeSpentIdlingOrEating;
                }
            }
        }
    }

    private void Roam()
    {
        navAgent.speed = walkSpeed;

        if (!walkPointSet && !moveTowardsTrap)
        {
            SearchWalkPoint();
        }
        else if (walkPointSet)
        {
            if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance + 0.1f)
            {
                walkPointSet = false;
                moveTowardsTrap = false;
                targetTrap = null;
                ChangeState(AnimalState.Idle);
                passiveTimer = timeSpentIdlingOrEating;
            }
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        Vector3 randomPoint = transform.position + new Vector3(randomX, 0f, randomZ);

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            walkPointSet = true;
            navAgent.SetDestination(walkPoint);
        }
    }

    public void GetTrapped(Transform trapTransform)
    {
        ChangeState(AnimalState.Injured);
        canWalk = false;
        navAgent.isStopped = true;
        navAgent.Warp(trapTransform.position);
    }

    private void ChangeState(AnimalState newState)
    {
        if (currentState == newState) return;

        animator.SetBool("Injured", false);
        animator.SetBool("Eating", false);
        animator.SetBool("Roaming", false);
        animator.SetBool("Fleeing", false);
        animator.SetBool("Idle", false);

        currentState = newState;
        animator.SetBool(newState.ToString(), true);
    }

    private void OnDrawGizmos()
    {
        if (raycastSource != null && player != null)
        {
            Vector3 directionToPlayer = (player.position - raycastSource.position).normalized;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            Gizmos.DrawRay(raycastSource.position, directionToPlayer * raycastLength);
        }
    }
}