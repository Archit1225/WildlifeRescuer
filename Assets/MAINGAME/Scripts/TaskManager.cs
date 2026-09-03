using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActiveTask
{
    public string taskName;
    public Transform targetTrap;
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
    public int dailyScore = 0;
    public List<ActiveTask> currentTasks = new List<ActiveTask>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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

    public void CreateTask(string name, Transform target, float timeAllowed, int bonusPoints)
    {
        ActiveTask newTask = new ActiveTask
        {
            taskName = name,
            targetTrap = target,
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
            dailyScore += totalPointsEarned;

            Debug.Log($"Task Completed! Earned: {totalPointsEarned} points.");

            CleanupTask(completedTask);
        }
    }

    private void FailTask(ActiveTask task)
    {
        Debug.Log($"Task {task.taskName} time expired! Daily score penalty applied.");
        dailyScore -= 50;
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
}