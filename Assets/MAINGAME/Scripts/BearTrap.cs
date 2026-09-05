using UnityEngine;

public class BearTrap : Trap 
{
    private Animator anim;
    public GameObject crowbarPrefab;
    public GameObject placePoint;
    public TimedDespawn timedDespawn;
    private Stag stag;

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
        TaskManager.Instance.CompleteTask(transform);
        
        anim.Play("UnTrap");
        stag.FreeFromTrap();
        Instantiate(crowbarPrefab, transform.position, transform.rotation);
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
                    //TaskManager.Instance.CreateTask("Bear Trap Rescue", transform, stag.animalData.speciesName, timeLimit, bonusPoints);
                }
            }
        }
    }
}