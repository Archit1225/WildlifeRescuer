using UnityEngine;

public class TimedDespawn : MonoBehaviour
{
    [Tooltip("Time in seconds before the object is destroyed")]
    public float lifetimeInSeconds = 120f; // 2 minutes = 120 seconds

    private void Start()
    {
        // The second parameter tells Unity how long to wait before executing the destroy command
        Destroy(gameObject, lifetimeInSeconds);
    }
}