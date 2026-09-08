using UnityEngine;

public class TrappedAnimalVisibility : MonoBehaviour
{
    [Header("Animal References")]
    [Tooltip("Drag the Renderer (or SkinnedMeshRenderer) of the animal here.")]
    public Renderer animalRenderer;

    [Header("Material Settings")]
    [Tooltip("Create a semi-transparent material and drag it here.")]
    public Material transparentMaterial;

    private Material[] originalMaterials;
    private bool isDisarmed = false;

    private void Start()
    {
        if (animalRenderer != null)
        {
            // Store the animal's original materials so we can restore them later
            originalMaterials = animalRenderer.sharedMaterials;
        }
        else
        {
            Debug.LogWarning($"[TrappedAnimalVisibility] No Renderer assigned on {gameObject.name}!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDisarmed)
        {
            SetAnimalTransparent(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isDisarmed)
        {
            SetAnimalTransparent(false);
        }
    }

    /// <summary>
    /// Call this function from your bear trap disarm/release script when the trap is solved.
    /// </summary>
    public void OnTrapDisarmed()
    {
        isDisarmed = true;
        SetAnimalTransparent(false); // Restore original solid materials
    }

    private void SetAnimalTransparent(bool makeTransparent)
    {
        if (animalRenderer == null) return;

        if (makeTransparent)
        {
            // Create an array matching the number of material slots, filled with the transparent material
            Material[] tempMaterials = new Material[animalRenderer.materials.Length];
            for (int i = 0; i < tempMaterials.Length; i++)
            {
                tempMaterials[i] = transparentMaterial;
            }
            animalRenderer.materials = tempMaterials;
        }
        else
        {
            // Revert back to original materials
            animalRenderer.materials = originalMaterials;
        }
    }
}