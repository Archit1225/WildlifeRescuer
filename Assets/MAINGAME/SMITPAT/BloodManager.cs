using UnityEngine;

public class BloodManager : MonoBehaviour
{
    private BloodNode[] bloodNodes;
    private bool clothPhaseComplete = false;
    private bool sprayPhaseComplete = false;
    private bool bandagePhaseComplete = false;
    public Stag stag;

    [Header("Scoring")]
    public int treatmentPoints = 50;

    private void Start()
    {
        bloodNodes = GetComponentsInChildren<BloodNode>(true);
    }

    private void Update()
    {
        if (!clothPhaseComplete)
        {
            CheckClothPhase();
        }
        else if (!sprayPhaseComplete)
        {
            CheckSprayPhase();
        }
        else if (!bandagePhaseComplete)
        {
            CheckBandaidPhase();
        }

        if (bandagePhaseComplete)
        {
            Debug.Log("Treatment Done!");
            stag.Healed();
            gameObject.SetActive(false);
        }
    }

    private void CheckClothPhase()
    {
        bool allClean = true;
        foreach (BloodNode node in bloodNodes)
        {
            if (!node.isCleaned) { allClean = false; break; }
        }

        if (allClean)
        {
            clothPhaseComplete = true;
            Debug.Log("All blood splats cleaned! Ready for Spray.");
        }
    }

    private void CheckSprayPhase()
    {
        bool allSprayed = true;
        foreach (BloodNode node in bloodNodes)
        {
            if (!node.isSprayed) { allSprayed = false; break; }
        }

        if (allSprayed)
        {
            sprayPhaseComplete = true;
            Debug.Log("All patches sprayed! Ready for Bandage phase.");
        }
    }

    private void CheckBandaidPhase()
    {
        bool allBandaged = true;
        foreach (BloodNode node in bloodNodes)
        {
            if (!node.isBandaged) { allBandaged = false; break; }
        }

        if (allBandaged)
        {
            bandagePhaseComplete = true;
            Debug.Log("All patches bandaged! Stag is now healed.");

            if (TaskManager.Instance != null && stag != null)
            {
                TaskManager.Instance.CompleteTreatment(stag.gameObject, treatmentPoints);
            }
        }
    }
}