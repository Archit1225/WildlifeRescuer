using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Tooltip("Type the exact name of your main game scene here")]
    public string mainSceneName = "MainScene";

    private void Start()
    {
        // Plays the start screen music immediately
        if (BGMManager.Instance != null) BGMManager.Instance.PlayStartMusic();
    }

    public void LoadMainGame()
    {
        SceneManager.LoadScene(mainSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}