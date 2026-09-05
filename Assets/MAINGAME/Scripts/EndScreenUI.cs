using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndScreenUI : MonoBehaviour
{
    [Header("UI Text References")]
    [SerializeField] private TMP_Text saveScoreText;
    [SerializeField] private TMP_Text treatScoreText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text finalScoreText;

    [Header("Scene Settings")]
    [SerializeField] private string gameplaySceneName = "MainGameScene"; // Replace with your exact gameplay scene name

    private void Start()
    {
        // Play the end screen music as soon as this screen loads
        if (BGMManager.Instance != null) BGMManager.Instance.PlayEndMusic();

        DisplayScores();
    }

    private void DisplayScores()
    {
        if (TaskManager.Instance != null)
        {
            int save = GameScoreData.saveScore;
            int treat = GameScoreData.treatScore;
            int combo = GameScoreData.GetCombo();
            int final = GameScoreData.CalculateFinalScore();

            if (saveScoreText != null) saveScoreText.text = $"Save Score: {save}";
            if (treatScoreText != null) treatScoreText.text = $"Treat Score: {treat}";
            if (comboText != null) comboText.text = $"Combo Multiplier: x{combo}";
            if (finalScoreText != null) finalScoreText.text = $"Total Score: {final}";
        }
        else
        {
            Debug.LogWarning("TaskManager instance not found! Make sure you tested by starting from the main game scene.");
        }
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}