using UnityEngine;

public class FishnetTrap : Trap
{
    [Header("Net Settings")]
    public GameObject netChild; // Drag the child net GameObject here
    public float dropHeight = 8f;

    [Header("Rope Cutting System")]
    public Renderer[] ropeRenderers; // Drag the 4 capsule collider children here
    private Material[] originalMaterials;
    private bool allRopesCut = false;
    private Stag trappedAnimal; // Caches the animal to easily free it later

    [Header("Task Settings")]
    public float timeLimit = 60f;
    public int bonusPoints = 50;

    protected override void Start()
    {
        base.Start(); // Keeps the TrapManager registration working

        if (netChild != null)
        {
            netChild.SetActive(false);
        }

        // Cache the starting materials of the 4 ropes so we can detect when the knife changes them
        if (ropeRenderers != null && ropeRenderers.Length > 0)
        {
            originalMaterials = new Material[ropeRenderers.Length];
            for (int i = 0; i < ropeRenderers.Length; i++)
            {
                originalMaterials[i] = ropeRenderers[i].sharedMaterial;
            }
        }
    }

    private void Update()
    {
        // Only check for cuts if the trap is active (animal caught) and ropes aren't already cut
        if (isActive || allRopesCut || ropeRenderers == null || ropeRenderers.Length == 0) return;

        bool allChanged = true;

        for (int i = 0; i < ropeRenderers.Length; i++)
        {
            // If any rope still matches its original material, they are not all cut yet
            if (ropeRenderers[i].sharedMaterial == originalMaterials[i])
            {
                allChanged = false;
                break;
            }
        }

        if (allChanged)
        {
            allRopesCut = true;
            Debug.Log("All 4 ropes have been cut! The net is destroyed.");
            OnNetRemoved();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        if (other.CompareTag("Animal"))
        {
            isActive = false;

            Stag animal = other.GetComponent<Stag>();
            if (animal != null)
            {
                trappedAnimal = animal; // Store the animal reference for when the net is removed
                animal.GetTrapped(transform);

                Vector3 calculatedScale = Vector3.one;

                if (animal.animalData != null)
                {
                    calculatedScale = animal.animalData.fishnetScale;
                    calculatedScale.y *= -1;
                }

                // Position the child net in world space directly above the animal and turn it on
                netChild.transform.position = animal.transform.position + (Vector3.up * dropHeight);
                netChild.transform.localScale = calculatedScale;
                netChild.SetActive(true);

                Rigidbody netRb = netChild.GetComponent<Rigidbody>();
                if (netRb == null)
                {
                    netRb = netChild.AddComponent<Rigidbody>();
                }

                //TaskManager.Instance.CreateTask("Fishnet Rescue", transform, animal.animalData.speciesName, timeLimit, bonusPoints);
            }
        }
    }

    public void OnNetRemoved()
    {
        TaskManager.Instance.CompleteTask(transform);

        if (trappedAnimal != null)
        {
            trappedAnimal.FreeFromTrap();
        }

        // Hide or destroy the net child now that the animal is free
        if (netChild != null)
        {
            Destroy(gameObject);
        }
    }
}