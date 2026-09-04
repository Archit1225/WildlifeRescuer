using UnityEngine;

public class SprayTool : MonoBehaviour
{
    [Header("Spray Settings")]
    [Tooltip("An empty GameObject placed at the tip of the spray bottle")]
    public Transform nozzle; 
    
    [Tooltip("How far the spray reaches")]
    public float sprayDistance = 2.0f;
    
    [Tooltip("How much 'whiteness' is added per raycast hit")]
    public float strengthPerHit = 1.0f;

    [Tooltip("Optional: Add a Unity Particle System here to shoot particles")]
    public ParticleSystem sprayParticles;

    [Header("Animation Settings")]
    [Tooltip("The Animator component on your spray bottle or nozzle")]
    public Animator sprayAnimator;
    
    [Tooltip("The exact name of the Trigger parameter in your Animator window")]
    public string animationTriggerName = "Spray";

    public void FireSpray()
    {
        if (sprayParticles != null) sprayParticles.Play();
        if (sprayAnimator != null) sprayAnimator.SetTrigger(animationTriggerName);

        Ray ray = new Ray(nozzle.position, nozzle.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, sprayDistance))
        {
            BloodNode node = hit.collider.GetComponent<BloodNode>();
            if (node != null)
            {
                node.ReceiveSpray(strengthPerHit);
                return; // Stop here if raycast succeeded
            }
        }

        Collider[] overlappingColliders = Physics.OverlapSphere(nozzle.position, 0.1f);
        foreach (Collider col in overlappingColliders)
        {
            BloodNode node = col.GetComponent<BloodNode>();
            if (node != null)
            {
                node.ReceiveSpray(strengthPerHit);
                break;
            }
        }
    }
}