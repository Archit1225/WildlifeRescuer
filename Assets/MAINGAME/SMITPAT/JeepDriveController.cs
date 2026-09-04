using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Rigidbody))]
public class JeepDriveController : MonoBehaviour
{
    [Header("Mount & Seat Setup")]
    [SerializeField] private Transform seatAnchor;
    [SerializeField] private Transform playerTrackingRoot;
    [SerializeField] private Transform playerHeadCamera;
    
    [Tooltip("MUST be a child of the Jeep in the Hierarchy!")]
    [SerializeField] private Transform exitPoint;
    
    [Tooltip("Distance from the SEAT (not the Jeep center) to enter")]
    [SerializeField] private float mountDistance = 3.0f;

    [Header("Drive Settings")]
    [SerializeField] private float forwardSpeed = 8f;
    [SerializeField] private float reverseSpeed = 4f;
    [SerializeField] private float turnTorque = 120f;

    private Rigidbody rb;
    private bool isMounted = false;

    private bool lastAPressed = false;
    private bool lastBPressed = false;
    
    // Store the original parent so we can restore it when exiting
    private Transform originalPlayerParent;

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

        if (!isMounted)
        {
            if (isAPressed && !lastAPressed)
            {
                // FIX 1: Check distance to the SEAT, not the Jeep's center
                if (playerTrackingRoot != null && seatAnchor != null)
                {
                    float distance = Vector3.Distance(playerTrackingRoot.position, seatAnchor.position);
                    if (distance <= mountDistance)
                    {
                        MountJeep();
                    }
                }
            }
        }
        else 
        {
            if (isBPressed && !lastBPressed)
            {
                DismountJeep();
            }
        }

        lastAPressed = isAPressed;
        lastBPressed = isBPressed;
    }

    public void MountJeep()
    {
        if (isMounted || playerTrackingRoot == null || seatAnchor == null) return;
        isMounted = true;

        if (playerTrackingRoot.TryGetComponent(out Rigidbody pRb))
        {
            pRb.isKinematic = true;
        }
        if (playerTrackingRoot.TryGetComponent(out Collider pCol))
        {
            pCol.enabled = false;
        }

        // FIX 2: Parent the player to the seat so Unity perfectly syncs their movement
        originalPlayerParent = playerTrackingRoot.parent;
        playerTrackingRoot.SetParent(seatAnchor, true);
    }

    public void DismountJeep()
    {
        if (!isMounted || playerTrackingRoot == null) return;
        isMounted = false;

        // Unparent the player
        playerTrackingRoot.SetParent(originalPlayerParent, true);

        if (exitPoint != null)
        {
            playerTrackingRoot.position = exitPoint.position;
            playerTrackingRoot.rotation = exitPoint.rotation;
        }
        else
        {
            playerTrackingRoot.position = transform.position + (-transform.right * 1.5f);
        }

        if (playerTrackingRoot.TryGetComponent(out Rigidbody pRb))
        {
            pRb.isKinematic = false;
        }
        if (playerTrackingRoot.TryGetComponent(out Collider pCol))
        {
            pCol.enabled = true;
        }
    }

    // FIX 3: Moved back to LateUpdate. VR Headsets update their tracking very late in the frame.
    // Doing this here prevents the headset from fighting the camera snap.
    private void LateUpdate()
    {
        if (isMounted && playerTrackingRoot != null && seatAnchor != null)
        {
            playerTrackingRoot.rotation = seatAnchor.rotation;

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