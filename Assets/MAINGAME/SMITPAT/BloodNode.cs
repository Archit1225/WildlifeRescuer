using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Renderer))]
public class BloodNode : MonoBehaviour
{
    [Header("Phase 1: Cleaning")]
    public float maxScrubTime = 3.0f;
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
    // PHASE 1: CLOTH SCRUBBING
    private void OnTriggerStay(Collider other)
    {
        if (isCleaned) return;

        if (other.CompareTag("Cloth"))
        {
            Rigidbody clothRb = other.attachedRigidbody;

            if (clothRb != null)
            {
                float speed = clothRb.linearVelocity.magnitude;

                if (speed > 0.1f)
                {
                    currentScrubValue += speed * Time.deltaTime;

                    float cleanPercent = currentScrubValue / maxScrubTime;
                    transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, cleanPercent);

                    if (currentScrubValue >= maxScrubTime)
                    {
                        isCleaned = true;

                        transform.localScale = initialScale;
                        if (outlineMaterial != null)
                        {
                            rend.material = outlineMaterial;
                            activeMaterial = rend.material;
                            activeMaterial.color = new Color(1f, 1f, 1f, 0.1f);
                        }
                    }
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
   [Header("Phase 3: Bandaging")]
    [Tooltip("Drag the matching bandaid child (e.g. bandaid_1) here")]
    public GameObject bandaidChildOnBody;
    public bool isBandaged = false;

    private void OnTriggerEnter(Collider other)
    {
        // Only allow bandaging if the spray phase is done
        if (isSprayed && !isBandaged)
        {
            // Check if the object touching us has the "Bandaid" tag
            if (other.CompareTag("Bandaid"))
            {
                isBandaged = true;

                // 1. Turn ON the bandaid child on the body
                if (bandaidChildOnBody != null)
                {
                    bandaidChildOnBody.SetActive(true);
                }

                // 2. Destroy the bandaid the player is holding in their hand
                // (attachedRigidbody.gameObject ensures we destroy the whole grabbable object, not just the collider)
                if (other.attachedRigidbody != null)
                {
                    Destroy(other.attachedRigidbody.gameObject);
                }
                else
                {
                    Destroy(other.gameObject);
                }

                // 3. Turn OFF this blood child
                gameObject.SetActive(false);
            }
        }
    }
}