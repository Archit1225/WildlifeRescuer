using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Rigidbody))]
public class JeepDriveController : MonoBehaviour
{
    [Header("Mount & Seat Setup")]
    [Tooltip("The empty Transform marking where the player's head/seat should be")]
    [SerializeField] private Transform seatAnchor;

    [Tooltip("The top-level parent of your VR rig (e.g. XR Origin or Auto Hand Player)")]
    [SerializeField] private Transform playerTrackingRoot;

    [Tooltip("The Main Camera (head) inside your VR rig")]
    [SerializeField] private Transform playerHeadCamera;

    [Tooltip("Spot placed outside the driver door where the player lands on exit")]
    [SerializeField] private Transform exitPoint;

    [Header("Drive Settings")]
    [SerializeField] private float forwardSpeed = 8f;
    [SerializeField] private float reverseSpeed = 4f;
    [SerializeField] private float turnTorque = 120f;

    private Rigidbody rb;
    private bool isMounted = false;
    private bool isPlayerInTriggerZone = false;

    private bool lastAPressed = false;
    private bool lastBPressed = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (playerHeadCamera == null && Camera.main != null)
        {
            playerHeadCamera = Camera.main.transform;
        }
    }

    private void Update()
    {
        var rightHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, 
            rightHandDevices
        );

        if (rightHandDevices.Count == 0) return;
        InputDevice rightController = rightHandDevices[0];

        rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool isAPressed);
        rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isBPressed);

        if (!isMounted && isPlayerInTriggerZone)
        {
            if (isAPressed && !lastAPressed)
            {
                MountJeep();
            }
        }
        else if (isMounted)
        {
            if (isBPressed && !lastBPressed)
            {
                DismountJeep();
            }
        }

        lastAPressed = isAPressed;
        lastBPressed = isBPressed;
    }

    private void LateUpdate()
    {
        if (!isMounted || playerTrackingRoot == null || seatAnchor == null) return;

        // Keep the player rotation matched to the Jeep
        playerTrackingRoot.rotation = seatAnchor.rotation;

        // Calculate headset offset so the player's actual EYES align with the seat anchor
        if (playerHeadCamera != null)
        {
            Vector3 headOffset = playerHeadCamera.position - playerTrackingRoot.position;
            playerTrackingRoot.position = seatAnchor.position - headOffset;
        }
        else
        {
            playerTrackingRoot.position = seatAnchor.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerTrackingRoot != null && (other.transform == playerTrackingRoot || other.transform.IsChildOf(playerTrackingRoot)))
        {
            isPlayerInTriggerZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerTrackingRoot != null && (other.transform == playerTrackingRoot || other.transform.IsChildOf(playerTrackingRoot)))
        {
            isPlayerInTriggerZone = false;
        }
    }

    public void MountJeep()
    {
        if (isMounted || playerTrackingRoot == null || seatAnchor == null) return;
        isMounted = true;
        isPlayerInTriggerZone = false;
    }

    public void DismountJeep()
    {
        if (!isMounted || playerTrackingRoot == null) return;
        isMounted = false;

        if (exitPoint != null)
        {
            playerTrackingRoot.position = exitPoint.position;
            playerTrackingRoot.rotation = exitPoint.rotation;
        }
        else
        {
            playerTrackingRoot.position = transform.position + (-transform.right * 1.5f);
        }
    }

    private void FixedUpdate()
    {
        if (!isMounted) return;

        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        float currentSpeed = moveInput >= 0 ? forwardSpeed : reverseSpeed;
        Vector3 targetVelocity = transform.forward * (moveInput * currentSpeed);
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

        float turnAmount = turnInput * turnTorque * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}