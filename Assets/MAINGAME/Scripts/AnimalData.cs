using UnityEngine;

[CreateAssetMenu(fileName = "New Animal Data", menuName = "Wildlife/Animal Data")]
public class AnimalData : ScriptableObject
{
    [Header("Identity")]
    public string speciesName;

    [Header("Movement Settings")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 5f;
    public float walkPointRange = 20f;
    public float fleeDistance = 20f;

    [Header("Detection Settings")]
    public float raycastLength = 8f;
    public float visualConeAngle = 60f;
    public float detectionRange = 10f;

    [Header("Behavior Timers")]
    public float timeSpentIdlingOrEating = 3f;

    [Header("Trap Reactions")]
    public Vector3 fishnetScale = Vector3.one;
}