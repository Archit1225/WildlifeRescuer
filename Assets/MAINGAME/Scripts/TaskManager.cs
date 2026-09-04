using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActiveTask
{
    public string taskName;
    public Transform targetTrap;
    public GameObject associatedAnimal; // ADDED: Tracks which animal this task belongs to
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
    public int saveScore = 0;  // Tracks points from traps/nets
    public int treatScore = 0; // Tracks points from medical tools
    public List<ActiveTask> currentTasks = new List<ActiveTask>();

    // HashSet automatically prevents duplicate entries so we only count unique animals
    private HashSet<GameObject> uniqueAnimalsAssisted = new HashSet<GameObject>();

    private void Awake()
    {
            if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps scores alive when changing scenes
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
        int uniqueCount = uniqueAnimalsAssisted.Count;
        return uniqueCount >= 3 ? uniqueCount : 1;
    }

    // UPDATED: Added 'GameObject animal' parameter so we know who is trapped
    public void CreateTask(string name, Transform target, GameObject animal, float timeAllowed, int bonusPoints)
    {
        ActiveTask newTask = new ActiveTask
        {
            taskName = name,
            targetTrap = target,
            associatedAnimal = animal, 
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

    // UPDATED: Calculates save score and logs the unique animal
    public void CompleteTask(Transform trapTransform)
    {
        ActiveTask completedTask = currentTasks.Find(t => t.targetTrap == trapTransform);

        if (completedTask != null)
        {
            float timePercent = Mathf.Clamp01(completedTask.timeRemaining / completedTask.timeLimit);
            int bonusEarned = Mathf.RoundToInt(completedTask.maxBonusPoints * timePercent);

            int totalPointsEarned = completedTask.basePoints + bonusEarned;
            saveScore += totalPointsEarned; // Add to saveScore instead of dailyScore

            // Add the animal to our unique list for the combo multiplier
            if (completedTask.associatedAnimal != null)
            {
                uniqueAnimalsAssisted.Add(completedTask.associatedAnimal);
            }

            Debug.Log($"Task Completed! Earned: {totalPointsEarned} save points.");
            CleanupTask(completedTask);
        }
    }

    // NEW: Call this from your medical tool scripts when an animal is fully treated
    public void CompleteTreatment(GameObject animal, int pointsEarned)
    {
        treatScore += pointsEarned;
        
        if (animal != null)
        {
            uniqueAnimalsAssisted.Add(animal);
        }
        Debug.Log($"Treatment Complete! Earned {pointsEarned} treat points.");
    }

    // NEW: Calculates the final math formula at the end of the day/level
    public int CalculateFinalScore()
    {
        int uniqueCount = uniqueAnimalsAssisted.Count;
        int combo = 1;

        // If 3 or more unique animals were helped, the combo becomes the number of animals (or set this to a fixed number like 2)
        if (uniqueCount >= 3)
        {
            combo = uniqueCount; 
        }

        int totalScore = (saveScore + treatScore) * combo;

        Debug.Log("--- END OF DAY RESULTS ---");
        Debug.Log($"Save Score: {saveScore}");
        Debug.Log($"Treat Score: {treatScore}");
        Debug.Log($"Unique Animals: {uniqueCount} (Combo: x{combo})");
        Debug.Log($"TOTAL SCORE: {totalScore}");

        return totalScore;
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
        uniqueAnimalsAssisted.Clear();
    }
}