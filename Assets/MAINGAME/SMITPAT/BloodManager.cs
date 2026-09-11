using System.Collections.Generic;
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

    [Header("Treatment Tips")]
    [TextArea] public string clothTip = "Step 1: Grab the cloth from your kit to clean all blood splats.";
    [TextArea] public string sprayTip = "Step 2: Use the antiseptic spray on all cleaned wounds.";
    [TextArea] public string bandageTip = "Step 3: Wrap bandages around all sprayed wounds to finish healing.";

    private bool playerIsNear = false;
    private ActiveTask currentActiveTask;
    public List<GameObject> objDel = new List<GameObject>();

    private void Start()
    {
        bloodNodes = GetComponentsInChildren<BloodNode>(true);
    }

    private void OnEnable()
    {
        objDel.Add(this.gameObject);
        currentActiveTask = TaskManager.Instance.CreateTask($"Free the {stag.animalData.name}", transform, stag.animalData.name, 180f, 200, objDel);
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
            
            // Clear the tip when treatment is fully finished
            if (TipManager.Instance != null && playerIsNear)
            {
                TipManager.Instance.HideTip();
            }

            stag.Healed();
            gameObject.SetActive(false);
        }
    }

    #region Proximity Detection for Tips
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            UpdateCurrentTip();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            if (TipManager.Instance != null)
            {
                TipManager.Instance.HideTip();
            }
        }
    }

    private void UpdateCurrentTip()
    {
        if (!playerIsNear || TipManager.Instance == null) return;

        if (!clothPhaseComplete)
        {
            TipManager.Instance.ShowTip(clothTip, transform); // Pass this animal's transform
        }
        else if (!sprayPhaseComplete)
        {
            TipManager.Instance.ShowTip(sprayTip, transform);
        }
        else if (!bandagePhaseComplete)
        {
            TipManager.Instance.ShowTip(bandageTip, transform);
        }
    }
    #endregion

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
            
            // Instantly update UI tip to Step 2 (Spray)
            UpdateCurrentTip();
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

            // Instantly update UI tip to Step 3 (Bandage)
            UpdateCurrentTip();
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
                TaskManager.Instance.CompleteTreatment(stag.animalData.speciesName, treatmentPoints, currentActiveTask);
            }
        }
    }
}