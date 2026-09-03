using UnityEngine;

public class BloodManager : MonoBehaviour
{
    private BloodNode[] bloodNodes;
    private bool clothPhaseComplete = false;
    private bool sprayPhaseComplete = false;

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
            Debug.Log("All blood splats cleaned! They have reappeared as outlines. Ready for Spray.");
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
            Debug.Log("All patches completely sprayed! Ready for Bandage phase.");
        }
    }
}