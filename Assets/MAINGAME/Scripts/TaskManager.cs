using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActiveTask
{
    public string taskName;
    public Transform targetTrap;
    public string associatedSpecies;
    public float timeLimit;
    public float timeRemaining;
    public int maxBonusPoints;
    public int basePoints = 100;
    public GameObject waypointInstance;
    public Stag stagReference; 
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

    public ActiveTask CreateTask(string name, Transform target, string speciesName, float timeAllowed, int bonusPoints, Stag linkedStag)
    {
        ActiveTask newTask = new ActiveTask
        {
            taskName = name,
            targetTrap = target,
            associatedSpecies = speciesName,
            timeLimit = timeAllowed,
            timeRemaining = timeAllowed,
            maxBonusPoints = bonusPoints,
            stagReference = linkedStag 
        };

        GameObject markerObj = Instantiate(waypointPrefab, waypointCanvas);
        WaypointMarker markerScript = markerObj.GetComponent<WaypointMarker>();
        markerScript.target = target;
        newTask.waypointInstance = markerObj;

        currentTasks.Add(newTask);
        //Debug.Log($"New Task: {name}. Get there fast!");

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
        Debug.Log($"Task {task.taskName} time expired! Animal lost, but no points deducted.");

        // Point penalty has been removed.

        if (task.stagReference != null)
        {
            Destroy(task.stagReference.gameObject);
        }

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