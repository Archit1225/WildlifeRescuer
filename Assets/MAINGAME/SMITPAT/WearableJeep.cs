using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Autohand;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsJeepController : MonoBehaviour
{
    [Header("Seat & Tracking")]
    [SerializeField] private Transform seatAnchor;
    [SerializeField] private Transform playerTrackingRoot; // MUST be the top-level AutoHandPlayer!
    [SerializeField] private Transform exitPoint;
    [SerializeField] private float mountDistance = 3.0f;

    [Header("Jeep Settings")]
    [SerializeField] private AutoHandPlayer playerController;
    [SerializeField] private float forwardSpeed = 8f;
    [SerializeField] private float reverseSpeed = 4f;
    [SerializeField] private float turnTorque = 120f;

    private Rigidbody jeepRb;
    private Rigidbody playerRb;
    private bool isMounted = false;
    private float originalWalkSpeed;

    private bool lastAPressed = false;
    private bool lastBPressed = false;

    private void Awake()
    {
        jeepRb = GetComponent<Rigidbody>();
        if (playerTrackingRoot != null)
        {
            playerRb = playerTrackingRoot.GetComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        // Check right controller for A/B button presses
        var rightHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, 
            rightHandDevices
        );

        if (rightHandDevices.Count > 0)
        {
            InputDevice rightController = rightHandDevices[0];
            rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool isAPressed);
            rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isBPressed);

            if (!isMounted && isAPressed && !lastAPressed)
            {
                if (playerTrackingRoot != null && seatAnchor != null)
                {
                    if (Vector3.Distance(playerTrackingRoot.position, seatAnchor.position) <= mountDistance)
                    {
                        MountJeep();
                    }
                }
            }
            else if (isMounted && isBPressed && !lastBPressed)
            {
                DismountJeep();
            }

            lastAPressed = isAPressed;
            lastBPressed = isBPressed;
        }
    }

    private void FixedUpdate()
    {
        if (!isMounted) return;

        // LEFT Controller: Forward/Backward
        float moveInput = 0f;
        var leftHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller, leftHandDevices);
        
        if (leftHandDevices.Count > 0)
        {
            leftHandDevices[0].TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftThumbstick);
            moveInput = leftThumbstick.y; 
        }

        // RIGHT Controller: Steering
        float turnInput = 0f;
        var rightHandDevices = new List<InputDevice>();
        // Using rightHandDevices list again (already initialized in Update, but local to this method here)
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, rightHandDevices);
        
        if (rightHandDevices.Count > 0)
        {
            rightHandDevices[0].TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightThumbstick);
            turnInput = rightThumbstick.x;
        }

        // Apply movement to the Jeep
        float currentSpeed = moveInput >= 0 ? forwardSpeed : reverseSpeed;
        Vector3 targetVelocity = transform.forward * (moveInput * currentSpeed);
        jeepRb.linearVelocity = new Vector3(targetVelocity.x, jeepRb.linearVelocity.y, targetVelocity.z);

        float turnAmount = turnInput * turnTorque * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
        jeepRb.MoveRotation(jeepRb.rotation * turnRotation);
    }

    public void MountJeep()
    {
        if (isMounted) return;
        isMounted = true;

        // 1. COMPLETELY turn off the AutoHandPlayer script so it stops fighting the car's movement
        if (playerController != null)
        {
            playerController.enabled = false;
            
            // As a failsafe, grab whatever Rigidbody is attached to the player controller and freeze it
            if (playerController.TryGetComponent(out Rigidbody pRb))
            {
                pRb.isKinematic = true;
            }
        }

        // 2. Parent the Master Rig (which keeps Camera and Hands perfectly synced) to the seat
        if (playerTrackingRoot != null && seatAnchor != null)
        {
            playerTrackingRoot.position = seatAnchor.position;
            playerTrackingRoot.rotation = seatAnchor.rotation;
            playerTrackingRoot.SetParent(seatAnchor, true);
        }
    }

    public void DismountJeep()
    {
        if (!isMounted) return;
        isMounted = false;

        // 1. Unparent the Master Rig
        if (playerTrackingRoot != null)
        {
            playerTrackingRoot.SetParent(null, true);

            // 2. Move to the Exit Point safely
            if (exitPoint != null)
            {
                playerTrackingRoot.position = exitPoint.position;
                playerTrackingRoot.rotation = exitPoint.rotation;
            }
        }

        // 3. Turn the AutoHandPlayer back on so you can walk normally again
        if (playerController != null)
        {
            playerController.enabled = true;

            if (playerController.TryGetComponent(out Rigidbody pRb))
            {
                pRb.isKinematic = false;
            }
        }
    }
}