using UnityEngine;
using UnityEngine.AI;

public class ResourceScatterer : MonoBehaviour
{
    [Header("Lonely Items (Logs, Heavy Branches)")]
    public GameObject[] lonelyPrefabs;
    public int lonelyAmount = 20;

    [Header("Grouped Items (Weeds, Grass, Mushrooms)")]
    public GameObject[] groupPrefabs;
    public int groupClustersAmount = 15;
    public int minItemsPerGroup = 4;
    public int maxItemsPerGroup = 5;
    public float groupSpreadRadius = 1.5f; 

    [Header("Scatter Area Settings")]
    public float scatterRadius = 50f;
    public LayerMask groundLayer;

    void Start()
    {
        if (lonelyPrefabs.Length > 0) ScatterLonelyItems();
        if (groupPrefabs.Length > 0) ScatterGroupItems();
    }

    private void ScatterLonelyItems()
    {
        int successfulSpawns = 0;
        int attempts = 0;
        int maxAttempts = lonelyAmount * 2;

        while (successfulSpawns < lonelyAmount && attempts < maxAttempts)
        {
            attempts++;
            Vector3 randomPoint = GetRandomPointInCircle(transform.position, scatterRadius);

            if (TryGetValidGroundHit(randomPoint, out RaycastHit groundHit))
            {
                SpawnItem(lonelyPrefabs, groundHit);
                successfulSpawns++;
            }
        }
    }

    private void ScatterGroupItems()
    {
        int successfulClusters = 0;
        int attempts = 0;
        int maxAttempts = groupClustersAmount * 2;

        while (successfulClusters < groupClustersAmount && attempts < maxAttempts)
        {
            attempts++;

            Vector3 clusterCenter = GetRandomPointInCircle(transform.position, scatterRadius);

            if (NavMesh.SamplePosition(clusterCenter, out NavMeshHit centerHit, 5f, NavMesh.AllAreas))
            {
                int itemsToSpawn = Random.Range(minItemsPerGroup, maxItemsPerGroup + 1);

                for (int i = 0; i < itemsToSpawn; i++)
                {
                    Vector3 itemPoint = GetRandomPointInCircle(centerHit.position, groupSpreadRadius);

                    if (TryGetValidGroundHit(itemPoint, out RaycastHit groundHit))
                    {
                        SpawnItem(groupPrefabs, groundHit);
                    }
                }
                successfulClusters++;
            }
        }
    }


    private Vector3 GetRandomPointInCircle(Vector3 center, float radius)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        return center + new Vector3(randomCircle.x, 0f, randomCircle.y);
    }

    private bool TryGetValidGroundHit(Vector3 targetPoint, out RaycastHit validHit)
    {
        validHit = new RaycastHit();

        if (NavMesh.SamplePosition(targetPoint, out NavMeshHit navHit, 3f, NavMesh.AllAreas))
        {
            Vector3 rayStart = navHit.position + Vector3.up * 2f;
            if (Physics.Raycast(rayStart, Vector3.down, out validHit, 5f, groundLayer))
            {
                return true;
            }
        }
        return false;
    }

    private void SpawnItem(GameObject[] prefabArray, RaycastHit groundHit)
    {
        GameObject prefabToSpawn = prefabArray[Random.Range(0, prefabArray.Length)];

        Quaternion randomSpin = Quaternion.Euler(0, Random.Range(0, 360), 0);

        Quaternion slopeAlignment = Quaternion.FromToRotation(Vector3.up, groundHit.normal);
        Quaternion finalRotation = slopeAlignment * randomSpin;

        Instantiate(prefabToSpawn, groundHit.point, finalRotation, transform);
    }
}