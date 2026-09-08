using UnityEngine;
using TMPro; // Make sure to include this for TextMeshPro

public class TipManager : MonoBehaviour
{
    public static TipManager Instance;

    [Header("UI References")]
    public GameObject tipPanel;          // The single background panel
    public TextMeshProUGUI tipText;      // The single text component

    private void Awake()
    {
        // Set up the Singleton so any script can access it easily
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Hide the panel by default at start
        HideTip();
    }

    public void ShowTip(string message)
    {
        tipText.text = message;
        tipPanel.SetActive(true);
    }

    public void HideTip()
    {
        tipPanel.SetActive(false);
    }
}