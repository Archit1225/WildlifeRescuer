using UnityEngine;

public class Trap : MonoBehaviour
{
    public bool isActive = true;

    // Both Bear Traps and Fishnets will automatically run this to register themselves
    protected virtual void Start()
    {
        if (TrapManager.Instance != null)
        {
            TrapManager.Instance.RegisterTrap(this);
        }
    }

    protected virtual void OnDestroy()
    {
        if (TrapManager.Instance != null)
        {
            TrapManager.Instance.UnregisterTrap(this);
        }
    }
}