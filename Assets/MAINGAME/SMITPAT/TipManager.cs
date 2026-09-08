using UnityEngine;
using TMPro;

public class TipManager : MonoBehaviour
{
    public static TipManager Instance;

    [Header("UI References")]
    public GameObject tipPanel;
    public TextMeshProUGUI tipText;

    [Header("VR Follow Settings")]
    [Tooltip("Drag Main Camera / CenterEyeAnchor here. If left empty, it auto-finds Camera.main.")]
    public Transform playerCamera; 
    
    // X: right(+)/left(-), Y: up(+)/down(-), Z: forward distance
    public Vector3 offset = new Vector3(0.3f, -0.2f, 1.2f); 
    public float followSpeed = 5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (tipPanel != null) tipPanel.SetActive(false);
    }

    private void Start()
    {
        // Auto-assign camera if unassigned in Inspector
        if (playerCamera == null && Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }
    }

    private void Update()
    {
        // Only move when the tip panel is active
        if (tipPanel != null && tipPanel.activeSelf)
        {
            // Backup check in case Camera was initialized late
            if (playerCamera == null && Camera.main != null)
            {
                playerCamera = Camera.main.transform;
            }

            if (playerCamera != null)
            {
                // Calculate position relative to where headset is looking
                Vector3 targetPosition = playerCamera.position + playerCamera.TransformDirection(offset);
                
                // Smoothly slide towards target position
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

                // Rotate to face the player
                transform.rotation = Quaternion.LookRotation(transform.position - playerCamera.position);
            }
        }
    }

    public void ShowTip(string message)
    {
        if (tipText != null) tipText.text = message;
        if (tipPanel != null) tipPanel.SetActive(true);
    }

    public void HideTip()
    {
        if (tipPanel != null) tipPanel.SetActive(false);
    }
}