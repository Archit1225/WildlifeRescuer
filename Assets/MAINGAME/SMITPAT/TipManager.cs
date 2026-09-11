using UnityEngine;
using TMPro;

public class TipManager : MonoBehaviour
{
    public static TipManager Instance;

    [Header("UI References")]
    public GameObject tipPanel;
    public TextMeshProUGUI tipText;

    [Header("World Anchor Settings")]
    public Transform playerCamera; 
    public Vector3 worldOffset = new Vector3(0f, 1.5f, 0f); // How high above the animal/trap it floats
    public float followSpeed = 10f;

    private Transform currentTarget;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (tipPanel != null) tipPanel.SetActive(false);
    }

    private void Start()
    {
        if (playerCamera == null && Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (tipPanel != null && tipPanel.activeSelf)
        {
            if (playerCamera == null && Camera.main != null)
            {
                playerCamera = Camera.main.transform;
            }

            // Follow the animal/trap position smoothly
            if (currentTarget != null)
            {
                Vector3 targetPos = currentTarget.position + worldOffset;
                transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
            }

            // Billboard: Always face the player camera
            if (playerCamera != null)
            {
                transform.LookAt(playerCamera);
                transform.Rotate(0, 180, 0); // Flips text so it reads correctly facing the player
            }
        }
    }

    public void ShowTip(string message, Transform targetTransform)
    {
        currentTarget = targetTransform;
        if (tipText != null) tipText.text = message;
        if (tipPanel != null) tipPanel.SetActive(true);

        // Snap instantly to position on open so it doesn't glide from far away
        if (currentTarget != null)
        {
            transform.position = currentTarget.position + worldOffset;
        }
    }

    public void HideTip()
    {
        if (tipPanel != null) tipPanel.SetActive(false);
        currentTarget = null;
    }
}