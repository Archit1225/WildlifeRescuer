using UnityEngine;

public class Trap : MonoBehaviour
{
    public bool isActive = true;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        if (TrapManager.Instance != null)
        {
            TrapManager.Instance.RegisterTrap(this);
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
                Stag stag = other.GetComponent<Stag>();
                if (stag != null)
                {
                    stag.GetTrapped(transform);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (TrapManager.Instance != null)
        {
            TrapManager.Instance.UnregisterTrap(this);
        }
    }
}