using UnityEngine;

public class BearTrap : Trap // Inherits from the base Trap class
{
    private Animator anim;
    public GameObject crowbarPrefab;
    public GameObject placePoint;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void OnLeverPlaced()
    {
        Debug.Log("Trap disarmed");
        TaskManager.Instance.CompleteTask(transform);
        anim.Play("UnTrap");
        Instantiate(crowbarPrefab, transform.position, transform.rotation);
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
                Stag stag = other.GetComponent<Stag>();
                if (stag != null)
                {
                    stag.GetTrapped(transform);
                }
            }
        }
    }
}