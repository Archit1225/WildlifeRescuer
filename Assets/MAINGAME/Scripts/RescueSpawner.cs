using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class RescueEvent
{
    public string eventName;
    public GameObject[] combinedPrefabs; // Array to hold multiple animals (e.g., Stag, Deer, Rabbit)
    public float minSpawnDistance;
    public float maxSpawnDistance;
}

public class RescueSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Spawning Rules")]
    public RescueEvent[] availableEvents;
    public float timeBetweenSpawns = 60f;

    private float timer;

    private void Start()
    {
        timer = 5f;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SpawnEmergency();
            timer = timeBetweenSpawns;
        }
    }

    private void SpawnEmergency()
    {
        if (availableEvents.Length == 0) return;

        RescueEvent selectedEvent = availableEvents[Random.Range(0, availableEvents.Length)];

        if (selectedEvent.combinedPrefabs.Length == 0)
        {
            Debug.LogWarning($"No prefabs assigned to event: {selectedEvent.eventName}");
            return;
        }

        GameObject prefabToSpawn = selectedEvent.combinedPrefabs[Random.Range(0, selectedEvent.combinedPrefabs.Length)];

        float randomDistance = Random.Range(selectedEvent.minSpawnDistance, selectedEvent.maxSpawnDistance);
        Vector2 randomDir = Random.insideUnitCircle.normalized;

        Vector3 rawSpawnPos = player.position + new Vector3(randomDir.x * randomDistance, 10f, randomDir.y * randomDistance);

        if (NavMesh.SamplePosition(rawSpawnPos, out NavMeshHit hit, 20f, NavMesh.AllAreas))
        {
            GameObject spawnedPrefab = Instantiate(prefabToSpawn, hit.position, Quaternion.identity);

            Stag animalScript = spawnedPrefab.GetComponentInChildren<Stag>();
            if (animalScript != null)
            {
                animalScript.isDynamicallySpawned = true;
            }

            Debug.Log($"Emergency! {selectedEvent.eventName} spawned {prefabToSpawn.name} {Mathf.RoundToInt(Vector3.Distance(player.position, hit.position))} meters away.");
        }
        else
        {
            Debug.LogWarning("Could not find safe ground. Skipping spawn to avoid getting stuck in a tree.");
        }
    }
}