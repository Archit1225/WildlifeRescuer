using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Tooltip("Type the exact name of your main game scene here")]
    public string mainSceneName = "MainGame";
    public string tutorialSceneName = "Tutorial";
    public string startSceneName = "Start";

    private void Start()
    {
        // Plays the start screen music immediately
        if (BGMManager.Instance != null) BGMManager.Instance.PlayStartMusic();
    }

    public void LoadMainGame()
    {
        SceneManager.LoadScene(mainSceneName);
    }
    public void LoadTutorialGame()
    {
        SceneManager.LoadScene(tutorialSceneName);
    }
    public void LoadStartScene()
    {
        SceneManager.LoadScene(startSceneName);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}