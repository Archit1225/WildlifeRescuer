using UnityEngine;

public class BandaidTool : MonoBehaviour
{
    [Header("Application Settings")]
    [Tooltip("If true, the physical bandaid disappears from the player's hand when applied.")]
    public bool consumeOnUse = true;

    [Tooltip("Optional: Sound to play when successfully placed on a wound.")]
    public AudioSource applySound;

    [Tooltip("Optional: Particle effect (like a little sparkle) when applied.")]
    public ParticleSystem applyParticles;

    // This gets called by the BloodNode when the bandage is successfully accepted
    public void OnApplied()
    {
        if (applySound != null)
        {
            // Unparent the audio source so it doesn't get cut off if the object is destroyed
            applySound.transform.SetParent(null);
            applySound.Play();
            Destroy(applySound.gameObject, applySound.clip.length);
        }

        if (applyParticles != null)
        {
            applyParticles.transform.SetParent(null);
            applyParticles.Play();
            Destroy(applyParticles.gameObject, 2f);
        }

        if (consumeOnUse)
        {
            // Auto Hands handles grabbable destruction safely, 
            // but disabling it first ensures physics drop cleanly
            gameObject.SetActive(false); 
            Destroy(gameObject);
        }
    }
}