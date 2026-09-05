using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class RescueEvent
{
    public string eventName;
    public GameObject[] combinedPrefabs; // Now an array to hold multiple variations (e.g., Bear Trap vs Net)
}

public class RescueSpawner : MonoBehaviour
{
    private Transform player;

    [Header("Spawning Rules")]
    public RescueEvent[] availableEvents;
    public float timeBetweenSpawns = 60f;

    private float timer;

    private void Start()
    {
        if (Camera.main != null)
        {
            player = Camera.main.transform;
        }
        else
        {
            Debug.LogError("Main Camera missing! Ensure your VR camera has the 'MainCamera' tag.");
        }

        timer = 5f;
    }

    private void Update()
    {
        if (player == null) return;

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

        if (selectedEvent.combinedPrefabs == null || selectedEvent.combinedPrefabs.Length == 0)
        {
            Debug.LogWarning($"Event {selectedEvent.eventName} has no prefabs assigned!");
            return;
        }

        GameObject prefabToSpawn = selectedEvent.combinedPrefabs[Random.Range(0, selectedEvent.combinedPrefabs.Length)];
        
        Stag prefabAnimalScript = prefabToSpawn.GetComponentInChildren<Stag>();
        if (prefabAnimalScript == null || prefabAnimalScript.animalData == null)
        {
            Debug.LogError("The selected prefab is missing a Stag script or AnimalData assignment!");
            return;
        }

        float minDistance = prefabAnimalScript.animalData.minSpawnDistance;
        float maxDistance = prefabAnimalScript.animalData.maxSpawnDistance;

        float randomDistance = Random.Range(minDistance, maxDistance);
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector3 rawSpawnPos = player.position + new Vector3(randomDir.x * randomDistance, 10f, randomDir.y * randomDistance);

        if (NavMesh.SamplePosition(rawSpawnPos, out NavMeshHit hit, 20f, NavMesh.AllAreas))
        {
            GameObject spawnedPrefab = Instantiate(prefabToSpawn, hit.position, Quaternion.identity);

            Stag spawnedAnimalScript = spawnedPrefab.GetComponentInChildren<Stag>();
            if (spawnedAnimalScript != null)
            {
                spawnedAnimalScript.isDynamicallySpawned = true;
            }

            Debug.Log($"Emergency! {selectedEvent.eventName} ({prefabAnimalScript.animalData.speciesName}) spawned {Mathf.RoundToInt(Vector3.Distance(player.position, hit.position))} meters away.");
        }
        else
        {
            Debug.LogWarning("Could not find safe ground. Skipping spawn to avoid getting stuck in a tree.");
        }
    }
}