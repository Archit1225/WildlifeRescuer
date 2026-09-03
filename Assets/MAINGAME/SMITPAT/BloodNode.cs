using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Renderer))]
public class BloodNode : MonoBehaviour
{
    [Header("Phase 1: Cleaning")]
    public float maxScrubTime = 3.0f;
    public float scrubSpeed = 1.0f;
    public float currentScrubValue = 0f;
    public bool isCleaned = false; // Cloth phase done

    [Header("Phase 2: Spraying")]
    public float maxSprayHits = 5f; // How many times raycast needs to hit to be fully white
    public float currentSprayHits = 0f;
    public bool isSprayed = false; // Spray phase done

    [Header("Visuals")]
    [Tooltip("The material it changes to after cleaning (e.g., colorless with white outline)")]
    public Material outlineMaterial;
    
    private Vector3 initialScale;
    private Renderer rend;
    private Material activeMaterial;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        initialScale = transform.localScale;
        rend = GetComponent<Renderer>();
    }

    // PHASE 1: CLOTH SCRUBBING
    private void OnTriggerStay(Collider other)
    {
        if (isCleaned) return;

        if (other.CompareTag("Cloth"))
        {
            currentScrubValue += scrubSpeed * Time.deltaTime;
            
            // Shrink down
            float cleanPercent = currentScrubValue / maxScrubTime;
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, cleanPercent);

            if (currentScrubValue >= maxScrubTime)
            {
                isCleaned = true;
                
                // Reappear at normal size for the spray phase
                transform.localScale = initialScale;
                
                // Swap to the outline material
                if (outlineMaterial != null)
                {
                    rend.material = outlineMaterial;
                    activeMaterial = rend.material;
                    // Set starting color (mostly transparent white)
                    activeMaterial.color = new Color(1f, 1f, 1f, 0.1f); 
                }
            }
        }
    }

    // PHASE 2: RECEIVING SPRAY
    public void ReceiveSpray(float sprayAmount)
    {
        // Only accept spray if cleaned, and stop accepting if fully sprayed
        if (!isCleaned || isSprayed) return;

        currentSprayHits += sprayAmount;
        float sprayPercent = Mathf.Clamp01(currentSprayHits / maxSprayHits);

        // Gradually turn the color completely solid white
        if (activeMaterial != null)
        {
            Color startColor = new Color(1f, 1f, 1f, 0.1f);
            Color fullyWhite = new Color(1f, 1f, 1f, 1f);
            activeMaterial.color = Color.Lerp(startColor, fullyWhite, sprayPercent);
        }

        if (currentSprayHits >= maxSprayHits)
        {
            isSprayed = true;
        }
    }
}