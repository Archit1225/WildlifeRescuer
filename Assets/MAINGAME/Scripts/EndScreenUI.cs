using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Change to using UnityEngine.UI if you are using standard UI Text instead of TextMeshPro

public class EndScreenUI : MonoBehaviour
{
    [Header("UI Text References")]
    [SerializeField] private TextMeshProUGUI saveScoreText;
    [SerializeField] private TextMeshProUGUI treatScoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("Scene Settings")]
    [SerializeField] private string gameplaySceneName = "MainGameScene"; // Replace with your exact gameplay scene name

    private void Start()
    {
        DisplayScores();
    }

    private void DisplayScores()
    {
        if (TaskManager.Instance != null)
        {
            int save = TaskManager.Instance.saveScore;
            int treat = TaskManager.Instance.treatScore;
            int combo = TaskManager.Instance.GetCombo();
            int final = TaskManager.Instance.CalculateFinalScore();

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

    // Hook this method up to your Retry Button's OnClick() event in the Inspector
    public void RetryGame()
    {
        if (TaskManager.Instance != null)
        {
            Destroy(TaskManager.Instance.gameObject); // Clean up the persistent manager for a fresh run
        }
        SceneManager.LoadScene(gameplaySceneName);
    }

    // Hook this method up to your Quit Button's OnClick() event in the Inspector
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}