using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActiveTask
{
    public string taskName;
    public Transform targetTrap; // Kept only so the Waypoint UI knows where to point
    public string associatedSpecies;
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
    public List<ActiveTask> currentTasks = new List<ActiveTask>();

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

    public ActiveTask CreateTask(string name, Transform target, string speciesName, float timeAllowed, int bonusPoints)
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

        return newTask;
    }

    public void CompleteTask(ActiveTask taskToComplete)
    {
        if (taskToComplete != null && currentTasks.Contains(taskToComplete))
        {
            float timePercent = Mathf.Clamp01(taskToComplete.timeRemaining / taskToComplete.timeLimit);
            int bonusEarned = Mathf.RoundToInt(taskToComplete.maxBonusPoints * timePercent);

            int totalPointsEarned = taskToComplete.basePoints + bonusEarned;
            GameScoreData.saveScore += totalPointsEarned;

            if (!string.IsNullOrEmpty(taskToComplete.associatedSpecies))
            {
                GameScoreData.uniqueSpeciesAssisted.Add(taskToComplete.associatedSpecies);
            }

            Debug.Log($"Task Completed! Earned: {totalPointsEarned} save points.");
            CleanupTask(taskToComplete);
        }
        else
        {
            Debug.LogWarning("Attempted to complete a task that doesn't exist or was already cleared.");
        }
    }

    public void CompleteTreatment(string speciesName, int pointsEarned)
    {
        GameScoreData.treatScore += pointsEarned;

        if (!string.IsNullOrEmpty(speciesName))
        {
            GameScoreData.uniqueSpeciesAssisted.Add(speciesName);
        }
        Debug.Log($"Treatment Complete! Earned {pointsEarned} treat points.");
    }

    private void FailTask(ActiveTask task)
    {
        Debug.Log($"Task {task.taskName} time expired! Penalty applied.");

        // Clamped using Mathf.Max so saveScore can never drop below 0
        GameScoreData.saveScore = Mathf.Max(0, GameScoreData.saveScore - 50);

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
        GameScoreData.ResetData();

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