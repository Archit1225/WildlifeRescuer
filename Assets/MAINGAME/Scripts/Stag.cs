using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Stag : MonoBehaviour
{
    [Header("Data Profile")]
    public AnimalData animalData; // Drag your ScriptableObject here in the Inspector!

    private NavMeshAgent navAgent;
    private Animator animator;
    public Transform player;

    public LayerMask groundLayer, playerLayer, obstacleLayer;

    //Patrolling
    private bool walkPointSet;
    private Vector3 walkPoint;

    //Detection
    public Transform raycastSource;
    public Transform trappedTrans;
    public GameObject bloodSpat;
    public bool trapStag;
    private bool moveTowardsTrap = false;
    private Trap targetTrap;

    //States
    public enum AnimalState { Trapped, Eating, Idle, Roaming, Fleeing, Injured }
    private AnimalState currentState;
    private bool canWalk;

    // Passive Timers
    private float passiveTimer;

    private void Awake()
    {
        player = Camera.main.transform;
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentState = AnimalState.Idle;

        navAgent.speed = animalData.walkSpeed;
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
            case AnimalState.Trapped:
                moveTowardsTrap = false;
                targetTrap = null;
                break;
            case AnimalState.Injured:
                HandleInjuredState();
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

    private void HandleInjuredState()
    {
        Debug.Log("Stag is injured! Waiting for player to apply medical treatment...");
        bloodSpat.SetActive(true);


        if (!navAgent.isStopped)
        {
            navAgent.isStopped = true;
        }
    }

    public void Healed()
    {
        canWalk = true;
        navAgent.isStopped = false;
        ChangeState(AnimalState.Fleeing);
        walkPointSet = false;
    }

    public void FreeFromTrap()
    {
        canWalk = true;
        navAgent.isStopped = false;

        float randomChance = Random.Range(0f, 1f);

        if (randomChance <= 0.5f)
        {
            // Not injured, run away
            ChangeState(AnimalState.Fleeing);
            walkPointSet = false;
            Debug.Log("Stag freed safely! Fleeing into the woods.");
        }
        else
        {
            // Injured, needs medical attention
            ChangeState(AnimalState.Injured);
        }
    }

    private void DecideNextPassiveAction()
    {
        float random = Random.Range(0f, 1f);

        if (random <= 0.3f && !trapStag)
        {
            ChangeState(AnimalState.Roaming);
        }
        else if (random <= 0.6f && !trapStag)
        {
            ChangeState(AnimalState.Eating);
            passiveTimer = animalData.timeSpentIdlingOrEating;
        }
        else if (random <= 0.9f && !trapStag)
        {
            ChangeState(AnimalState.Idle);
            passiveTimer = animalData.timeSpentIdlingOrEating;
        }
        else
        {
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
        if (currentState == AnimalState.Trapped) return;

        Vector3 directionToPlayer = (player.position - raycastSource.position).normalized;

        if (Vector3.Distance(player.position, transform.position) <= animalData.detectionRange)
        {
            float angleToPlayer = Vector3.Angle(raycastSource.forward, directionToPlayer);

            if (angleToPlayer <= animalData.visualConeAngle / 2f)
            {
                if (!Physics.Raycast(raycastSource.position, directionToPlayer, animalData.raycastLength, obstacleLayer))
                {
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
        navAgent.speed = animalData.runSpeed;

        if (!walkPointSet)
        {
            Vector3 directionToPlayer = (transform.position - player.position).normalized;
            Vector3 runPoint = transform.position + (directionToPlayer * animalData.fleeDistance);

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

                if (Vector3.Distance(player.position, transform.position) > animalData.detectionRange)
                {
                    ChangeState(AnimalState.Idle);
                    passiveTimer = animalData.timeSpentIdlingOrEating;
                }
            }
        }
    }

    private void Roam()
    {
        navAgent.speed = animalData.walkSpeed;

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
                passiveTimer = animalData.timeSpentIdlingOrEating;
            }
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-animalData.walkPointRange, animalData.walkPointRange);
        float randomX = Random.Range(-animalData.walkPointRange, animalData.walkPointRange);

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
        ChangeState(AnimalState.Trapped);
        canWalk = false;
        navAgent.isStopped = true;
        navAgent.Warp(trapTransform.position);
        trapTransform.position = trappedTrans.position;
        TaskManager.Instance.CreateTask($"Free the {animalData.name}", transform, animalData.name, 180f, 200);
    }

    private void ChangeState(AnimalState newState)
    {
        if (currentState == newState) return;

        animator.SetBool("Injured", false);
        animator.SetBool("Trapped", false);
        animator.SetBool("Eating", false);
        animator.SetBool("Roaming", false);
        animator.SetBool("Fleeing", false);
        animator.SetBool("Idle", false);

        currentState = newState;
        animator.SetBool(newState.ToString(), true);
    }

    private void OnDrawGizmos()
    {
        if (raycastSource != null && player != null && animalData != null)
        {
            Vector3 directionToPlayer = (player.position - raycastSource.position).normalized;
            Gizmos.DrawWireSphere(transform.position, animalData.detectionRange);
            Gizmos.DrawRay(raycastSource.position, directionToPlayer * animalData.raycastLength);
        }
    }
}