using UnityEngine;

public class BearTrap : Trap 
{
    private Animator anim;
    public GameObject crowbarPrefab;
    public GameObject placePoint;
    public TrappedAnimalVisibility invisibility;
    
    [Header("Task Settings")]
    public float timeLimit = 90f;
    public int bonusPoints = 50;

    private Stag animalStag;
    private ActiveTask currentActiveTask; // TRACKS THE TASK INSTANCE

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void OnLeverPlaced()
    {
        Debug.Log("Trap disarmed");
        
        // 1. COMPLETE THE TASK USING THE SAVED REFERENCE
        if (TaskManager.Instance != null && currentActiveTask != null)
        {
            TaskManager.Instance.CompleteTask(currentActiveTask);
        }

        // 2. Hide and complete the world-space tip when the trap is solved
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

        // 3. FREE THE ANIMAL
        if (animalStag != null)
        {
            animalStag.FreeFromTrap();
        }

        //Instantiate(crowbarPrefab, transform.position, transform.rotation);
        
        if (placePoint != null)
        {
            placePoint.SetActive(false);
        }

        Destroy(gameObject);
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
                
                animalStag = other.GetComponent<Stag>();
                if (animalStag != null)
                {
                    animalStag.GetTrapped(transform);

                    // 4. CREATE TASK & SAVE THE REFERENCE
                    if (TaskManager.Instance != null)
                    {
                        currentActiveTask = TaskManager.Instance.CreateTask(
                            $"Free the {animalStag.animalData.name}", 
                            transform, 
                            animalStag.animalData.name, 
                            timeLimit, 
                            bonusPoints,
                            animalStag
                        );
                    }
                }
            }
        }
    }
}