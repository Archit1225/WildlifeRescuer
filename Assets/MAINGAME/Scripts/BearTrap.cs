using UnityEngine;

public class BearTrap : Trap 
{
    private Animator anim;
    public GameObject crowbarPrefab;
    public GameObject placePoint;
    public TrappedAnimalVisibility invisibility;
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
        
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.CompleteTask(currentActiveTask);
        }

        // Hide and complete the world-space tip when the trap is solved
        TaskTipTrigger tipTrigger = GetComponent<TaskTipTrigger>();
        if (tipTrigger != null)
        {
            tipTrigger.CompleteTask();
        }
        else if (TipManager.Instance != null)
        {
            TipManager.Instance.HideTip();
        }
        
        if (invisibility != null)
        {
            invisibility.OnTrapDisarmed();
        }

        if (anim != null)
        {
            anim.Play("UnTrap");
        }

        if (stag != null)
        {
            stag.FreeFromTrap();
        }

        Instantiate(crowbarPrefab, transform.position, transform.rotation);
        
        if (placePoint != null)
        {
            placePoint.SetActive(false);
        }
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
                if (placePoint != null) placePoint.SetActive(true);
                
                stag = other.GetComponent<Stag>();
                if (stag != null)
                {
                    stag.GetTrapped(transform);

                    if (TaskManager.Instance != null)
                    {
                        currentActiveTask = TaskManager.Instance.CreateTask($"Free the {stag.animalData.name}", transform, stag.animalData.name, timeLimit, bonusPoints);
                    }
                }
            }
        }
    }
}