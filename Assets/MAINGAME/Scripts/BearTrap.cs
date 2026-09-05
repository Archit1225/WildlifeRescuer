using UnityEngine;

public class BearTrap : Trap 
{
    private Animator anim;
    public GameObject crowbarPrefab;
    public GameObject placePoint;
    public TimedDespawn timedDespawn;
    private Stag stag;
    private ActiveTask currentActiveTask;

    [Header("Task Settings")]
    public float timeLimit = 90f;
    public int bonusPoints = 50;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void OnLeverPlaced()
    {
        Debug.Log("Trap disarmed");
        TaskManager.Instance.CompleteTask(currentActiveTask);
        
        anim.Play("UnTrap");
        stag.FreeFromTrap();
        Instantiate(crowbarPrefab, transform.position, transform.rotation);
        placePoint.SetActive(false);
        timedDespawn.enabled = true;        //Destroy(gameObject);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        if (other.CompareTag("Animal") || other.CompareTag("Stone"))
        {
            isActive = false;
            if (anim != null) anim.Play("Trap");

            if (other.CompareTag("Animal"))
            {
                placePoint.SetActive(true);
                stag = other.GetComponent<Stag>();
                if (stag != null)
                {
                    stag.GetTrapped(transform);

                    currentActiveTask = TaskManager.Instance.CreateTask($"Free the {stag.animalData.name}", transform, stag.animalData.name, 180f, 200);
                    //TaskManager.Instance.CreateTask("Bear Trap Rescue", transform, stag.animalData.speciesName, timeLimit, bonusPoints);
                }
            }
        }
    }
}