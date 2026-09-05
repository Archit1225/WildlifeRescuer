using UnityEngine;
using UnityEngine.XR;

public class HotbarController : MonoBehaviour
{
    [Header("VR Setup")]
    public Transform vrCamera;         
    public GameObject hotbarCanvas;    

    [Header("Follow Settings")]
    public Vector3 offset = new Vector3(0f, -0.4f, 0.6f); 
    public float followSpeed = 5f;

    [Header("Item Spawning")]
    public GameObject[] itemPrefabs;   
    public float dropDistanceBelowHotbar = 0.3f; // Spawns items 30cm below the UI

    private bool wasXPressed = false;

    private void Update()
    {
        CheckXButtonInput();

        if (!hotbarCanvas.activeSelf) return;

        // Force the CANVAS ITSELF to move, no matter how your hierarchy is set up
        Vector3 targetPos = vrCamera.position + vrCamera.TransformDirection(offset);
        hotbarCanvas.transform.position = Vector3.Lerp(hotbarCanvas.transform.position, targetPos, Time.deltaTime * followSpeed);
        
        // Make the canvas look in the same direction you are looking
        hotbarCanvas.transform.rotation = Quaternion.Euler(0, vrCamera.eulerAngles.y, 0);
    }

    private void CheckXButtonInput()
    {
        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (leftHand.TryGetFeatureValue(CommonUsages.primaryButton, out bool isPressed))
        {
            if (isPressed && !wasXPressed)
            {
                hotbarCanvas.SetActive(!hotbarCanvas.activeSelf);
            }
            wasXPressed = isPressed;
        }
    }

    public void SpawnItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < itemPrefabs.Length)
        {
            // Automatically calculate a position right beneath the floating UI
            Vector3 spawnLocation = hotbarCanvas.transform.position + (Vector3.down * dropDistanceBelowHotbar);
            Instantiate(itemPrefabs[slotIndex], spawnLocation, Quaternion.identity);
        }
    }
}