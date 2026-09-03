using UnityEngine;

public class FishnetTrap : Trap
{
    [Header("Net Settings")]
    public GameObject netPrefab;
    public float dropHeight = 8f;

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

                Vector3 calculatedScale = Vector3.one; // Fallback

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
            }
        }
    }
}