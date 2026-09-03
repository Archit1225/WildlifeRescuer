using System.Collections.Generic;
using UnityEngine;

public class TrapManager : MonoBehaviour
{
    public static TrapManager Instance { get; private set; }

    private List<Trap> activeTraps = new List<Trap>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterTrap(Trap trap)
    {
        if (!activeTraps.Contains(trap))
        {
            activeTraps.Add(trap);
        }
    }

    public void UnregisterTrap(Trap trap)
    {
        if (activeTraps.Contains(trap))
        {
            activeTraps.Remove(trap);
        }
    }

    public Trap GetRandomActiveTrapInRadius(Vector3 position, float radius)
    {
        List<Trap> nearbyTraps = new List<Trap>();

        foreach (Trap trap in activeTraps)
        {
            if (trap.isActive && Vector3.Distance(position, trap.transform.position) <= radius)
            {
                nearbyTraps.Add(trap);
            }
        }

        if (nearbyTraps.Count > 0)
        {
            return nearbyTraps[Random.Range(0, nearbyTraps.Count)];
        }

        return null;
    }
}