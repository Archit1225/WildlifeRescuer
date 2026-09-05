using UnityEngine;

public class FishnetTrap : Trap
{
    [Header("Net Settings")]
    public GameObject netPrefab;
    public float dropHeight = 8f;

    [Header("Task Settings")]
    public float timeLimit = 60f;
    public int bonusPoints = 50;

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        if (other.CompareTag("Animal"))
        {
            isActive = false;

            Stag animal = other.GetComponent<Stag>();
            if (animal != null)
            {
                animal.GetTrapped(transform);

                Vector3 calculatedScale = Vector3.one; 

                if (animal.animalData != null)
                {
                    calculatedScale = animal.animalData.fishnetScale;
                    calculatedScale.y *= -1;
                }

                Vector3 spawnPosition = animal.transform.position + (Vector3.up * dropHeight);
                GameObject spawnedNet = Instantiate(netPrefab, spawnPosition, Quaternion.identity);

                spawnedNet.transform.localScale = calculatedScale;

                Rigidbody netRb = spawnedNet.GetComponent<Rigidbody>();
                if (netRb == null)
                {
                    netRb = spawnedNet.AddComponent<Rigidbody>();
                }

                TaskManager.Instance.CreateTask("Fishnet Rescue", transform, animal.animalData.speciesName, timeLimit, bonusPoints);
            }
        }
    }

    // Call this from whatever VR interaction removes the net!
    public void OnNetRemoved()
    {
        TaskManager.Instance.CompleteTask(transform);
        // Add your logic here to free the animal and destroy the net
    }
}