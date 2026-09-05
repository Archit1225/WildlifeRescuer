using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActiveTask
{
    public string taskName;
    public Transform targetTrap;
    public string associatedSpecies; // CHANGED: Now tracks the species string, not the GameObject
    public float timeLimit;
    public float timeRemaining;
    public int maxBonusPoints;
    public int basePoints = 100;
    public GameObject waypointInstance;
}

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    [Header("UI References")]
    public GameObject waypointPrefab;
    public Transform waypointCanvas;

    [Header("Game State")]
    public int saveScore = 0;
    public int treatScore = 0;
    public List<ActiveTask> currentTasks = new List<ActiveTask>();

    // CHANGED: Now strictly checks for unique string names (e.g., "Stag", "Rabbit")
    private HashSet<string> uniqueSpeciesAssisted = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GameScoreData.ResetData();  
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        for (int i = currentTasks.Count - 1; i >= 0; i--)
        {
            ActiveTask task = currentTasks[i];
            task.timeRemaining -= Time.deltaTime;

            if (task.timeRemaining <= 0)
            {
                FailTask(task);
            }
        }
    }

    public int GetCombo()
    {
        int uniqueCount = uniqueSpeciesAssisted.Count;
        return uniqueCount >= 3 ? uniqueCount : 1;
    }

    // CHANGED: Replaced GameObject parameter with string speciesName
    public void CreateTask(string name, Transform target, string speciesName, float timeAllowed, int bonusPoints)
    {
        ActiveTask newTask = new ActiveTask
        {
            taskName = name,
            targetTrap = target,
            associatedSpecies = speciesName,
            timeLimit = timeAllowed,
            timeRemaining = timeAllowed,
            maxBonusPoints = bonusPoints
        };

        GameObject markerObj = Instantiate(waypointPrefab, waypointCanvas);
        WaypointMarker markerScript = markerObj.GetComponent<WaypointMarker>();
        markerScript.target = target;
        newTask.waypointInstance = markerObj;

        currentTasks.Add(newTask);
        Debug.Log($"New Task: {name}. Get there fast!");
    }

    public void CompleteTask(Transform trapTransform)
    {
        ActiveTask completedTask = currentTasks.Find(t => t.targetTrap == trapTransform);
        if (completedTask != null)
        {
            float timePercent = Mathf.Clamp01(completedTask.timeRemaining / completedTask.timeLimit);
            int bonusEarned = Mathf.RoundToInt(completedTask.maxBonusPoints * timePercent);

            int totalPointsEarned = completedTask.basePoints + bonusEarned;
            GameScoreData.saveScore += totalPointsEarned;

            // Log the unique species string
            if (!string.IsNullOrEmpty(completedTask.associatedSpecies))
            {
                uniqueSpeciesAssisted.Add(completedTask.associatedSpecies);
            }

            Debug.Log($"Task Completed! Earned: {totalPointsEarned} save points.");
            CleanupTask(completedTask);
        }
    }

    public void CompleteTreatment(string speciesName, int pointsEarned)
    {
        GameScoreData.treatScore += pointsEarned;

        if (!string.IsNullOrEmpty(speciesName))
        {
            uniqueSpeciesAssisted.Add(speciesName);
        }
        Debug.Log($"Treatment Complete! Earned {pointsEarned} treat points.");
    }

    private void FailTask(ActiveTask task)
    {
        Debug.Log($"Task {task.taskName} time expired! Penalty applied.");
        saveScore -= 50;
        CleanupTask(task);
    }

    private void CleanupTask(ActiveTask task)
    {
        if (task.waypointInstance != null)
        {
            Destroy(task.waypointInstance);
        }
        currentTasks.Remove(task);
    }

    public void ResetForNextDay()
    {
        saveScore = 0;
        treatScore = 0;
        uniqueSpeciesAssisted.Clear();

        foreach (ActiveTask task in currentTasks)
        {
            if (task.waypointInstance != null)
            {
                Destroy(task.waypointInstance);
            }
        }

        currentTasks.Clear();
    }
}