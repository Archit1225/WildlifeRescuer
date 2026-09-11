using UnityEngine;
using UnityEngine.SceneManagement;

public class DayNightCycle2 : MonoBehaviour
{
    [Header("Timer Settings")]
    public float dayLengthInSeconds = 120f;
    [Range(0f, 1f)] public float timeOfDay = 0.25f; // 0 = midnight, 0.5 = noon

    [Header("Scene Transition")]
    public string scoreScene = "ScoreScene";

    private bool isNightMusicPlaying = false;

    private void Awake()
    {
        // Wipe the static score bus clean the moment the scene starts
        GameScoreData.ResetData();
    }

    private void Start()
    {
        if (BGMManager.Instance != null)
            BGMManager.Instance.PlayDayMusic();
    }

    private void Update()
    {
        timeOfDay += Time.deltaTime / dayLengthInSeconds;

        // Switch to night background music
        if (timeOfDay >= 0.75f && !isNightMusicPlaying)
        {
            isNightMusicPlaying = true;
            if (BGMManager.Instance != null)
                BGMManager.Instance.PlayNightMusic();
        }

        // Trigger end of day
        if (timeOfDay >= 1f)
        {
            timeOfDay = 1f;
            TriggerEndOfDay();
        }
    }

    private void TriggerEndOfDay()
    {
        Debug.Log("Midnight reached! Loading End Screen...");
        SceneManager.LoadScene(scoreScene);
    }
}