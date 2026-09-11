using NUnit.Framework;
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
    public List<GameObject> objs2Del = new List<GameObject>(); // Modified to accept an array
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

    // Modified parameter: GameObject[] objectsToDelete
    public ActiveTask CreateTask(string name, Transform target, string speciesName, float timeAllowed, int bonusPoints, List<GameObject> objectsToDelete)
    {
        ActiveTask newTask = new ActiveTask
        {
            taskName = name,
            targetTrap = target,
            associatedSpecies = speciesName,
            timeLimit = timeAllowed,
            timeRemaining = timeAllowed,
            maxBonusPoints = bonusPoints,
            objs2Del = objectsToDelete
        };

        GameObject markerObj = Instantiate(waypointPrefab, waypointCanvas);
        WaypointMarker markerScript = markerObj.GetComponent<WaypointMarker>();
        markerScript.target = target;
        newTask.waypointInstance = markerObj;

        currentTasks.Add(newTask);

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

    public void CompleteTreatment(string speciesName, int pointsEarned, ActiveTask taskToComplete)
    {
        GameScoreData.treatScore += pointsEarned;

        if (!string.IsNullOrEmpty(speciesName))
        {
            GameScoreData.uniqueSpeciesAssisted.Add(speciesName);
        }
        Debug.Log($"Treatment Complete! Earned {pointsEarned} treat points.");

        if (taskToComplete != null && currentTasks.Contains(taskToComplete))
        {
            CleanupTask(taskToComplete);
        }
    }

    private void FailTask(ActiveTask task)
    {
        Debug.Log($"Task {task.taskName} time expired! Animal lost, but no points deducted.");

        // Modified to loop through the array and destroy each object safely
        if (task.objs2Del != null)
        {
            foreach (GameObject obj in task.objs2Del)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
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