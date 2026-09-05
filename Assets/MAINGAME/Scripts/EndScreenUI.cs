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
    [SerializeField] private string gameplaySceneName = "MainGameScene"; 

    private void Start()
    {
        if (BGMManager.Instance != null) BGMManager.Instance.PlayEndMusic();

        DisplayScores();
    }

    private void DisplayScores()
    {
        // Using your correct class name: GameScoreData
        int save = GameScoreData.saveScore;
        int treat = GameScoreData.treatScore;
        int combo = GameScoreData.GetCombo();
        int final = GameScoreData.CalculateFinalScore();

        if (saveScoreText != null) saveScoreText.text = $"Save Score: {save}";
        if (treatScoreText != null) treatScoreText.text = $"Treat Score: {treat}";
        if (comboText != null) comboText.text = $"Combo Multiplier: x{combo}";
        if (finalScoreText != null) finalScoreText.text = $"Total Score: {final}";
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