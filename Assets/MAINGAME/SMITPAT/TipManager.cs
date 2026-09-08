using UnityEngine;
using TMPro;

public class TipManager : MonoBehaviour
{
    public static TipManager Instance;

    [Header("UI References")]
    public GameObject tipPanel;
    public TextMeshProUGUI tipText;

    [Header("VR Follow Settings")]
    public Transform playerCamera; // Drag your VR Main Camera here
    // X: right/left, Y: up/down, Z: forward distance
    public Vector3 offset = new Vector3(0.4f, -0.3f, 1.0f); 
    public float followSpeed = 5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        tipPanel.SetActive(false); // Hide at start
    }

    private void Update()
    {
        if (tipPanel.activeSelf && playerCamera != null)
        {
            // Calculate where the UI should float relative to where the player is looking
            Vector3 targetPosition = playerCamera.position + playerCamera.TransformDirection(offset);
            
            // Smoothly glide towards that position
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

            // Rotate to always face the player
            transform.rotation = Quaternion.LookRotation(transform.position - playerCamera.position);
        }
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