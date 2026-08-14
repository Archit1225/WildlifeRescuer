using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class Stag : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public Animator animator;
    public Transform player;

    public LayerMask groundLayer, playerLayer, obstacleLayer;

    //Animal Properties
    [SerializeField] private float moveSpeed;

    //Patrolling
    public bool walkPointSet;
    public Vector3 walkPoint;
    public float walkPointRange;

    //Detection
    public float raycastLength;
    public Transform raycastSource;
    public float visualConeAngle;
    public float detectionRange;

    //States
    public enum AnimalState { Injured, Eating, Roaming, Fleeing, Attacking}
    private AnimalState currentState;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        navAgent = GetComponent<NavMeshAgent>();    
        animator = GetComponent<Animator>();

        navAgent.speed = moveSpeed; 
    }

    private void Update()
    {
        if (Vector3.Distance(player.position, transform.position) <= detectionRange) {

            if (Vector3.Dot(raycastSource.forward, player.position - transform.position) < visualConeAngle)
            {
                Vector3 direction = player.position - raycastSource.position;
                if (Physics.Raycast(raycastSource.transform.position, direction, out RaycastHit hit, raycastLength, obstacleLayer))
                {
                    ChangeState(AnimalState.Fleeing);
                    Flee();
                }
            }
        }


    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(randomZ, randomX);  

        if(Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
        {
            walkPointSet = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Traps"))
        {
            ChangeState(AnimalState.Injured);
            //Stop Moving
            //Hut Audio
        }
    }

    private void Flee()
    {

    }

    private void Roam()
    {
        if (!walkPointSet) { SearchWalkPoint(); }

        if(walkPointSet)
        {
            navAgent.SetDestination(walkPoint);
        }
        Vector3 distanceToWalkPoint = walkPoint - transform.position;   
        if(distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;   
        }
    }


    private void ChangeState(AnimalState newState)
    {
        if(currentState == AnimalState.Injured) animator.SetBool("Injured", false);
        if(currentState == AnimalState.Eating) animator.SetBool("Eating", false);
        if(currentState == AnimalState.Roaming) animator.SetBool("Roaming", false);
        if(currentState == AnimalState.Fleeing) animator.SetBool("Fleeing", false);
        if(currentState == AnimalState.Attacking) animator.SetBool("Attacking", false);

        currentState = newState;

        if(currentState == AnimalState.Injured) animator.SetBool("Injured", true);
        if(currentState == AnimalState.Eating) animator.SetBool("Eating", true);
        if(currentState == AnimalState.Roaming) animator.SetBool("Roaming", true);
        if(currentState == AnimalState.Fleeing) animator.SetBool("Fleeing", true);
        if(currentState == AnimalState.Attacking) animator.SetBool("Attacking", true);
    }
}
