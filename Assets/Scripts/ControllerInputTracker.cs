using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ControllerInputTracker : MonoBehaviour
{
    private InputDevice leftController;
    private InputDevice rightController;

    void Start()
    {
        TryInitializeControllers();
        InputDevices.deviceConnected += OnDeviceConnected;
        InputDevices.deviceDisconnected += OnDeviceDisconnected;
    }

    void OnDestroy()
    {
        InputDevices.deviceConnected -= OnDeviceConnected;
        InputDevices.deviceDisconnected -= OnDeviceDisconnected;
    }

    private void OnDeviceConnected(InputDevice device)
    {
        TryInitializeControllers();
    }

    private void OnDeviceDisconnected(InputDevice device)
    {
        TryInitializeControllers();
    }

    private void TryInitializeControllers()
    {
        var leftDevices = new List<InputDevice>();
        var rightDevices = new List<InputDevice>();

        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftDevices);
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightDevices);

        leftController = leftDevices.Count > 0 ? leftDevices[0] : new InputDevice();
        rightController = rightDevices.Count > 0 ? rightDevices[0] : new InputDevice();
    }

    void Update()
    {
        bool primaryButton = false;      // A (right) or X (left)
        bool secondaryButton = false;    // B (right) or Y (left)
        bool xButton = false, yButton = false;
        bool triggerPressed = false;
        bool leftTrigger = false;
        bool gripPressed = false, leftGrip = false;
        Vector2 leftStick = Vector2.zero;
        Vector2 rightStick = Vector2.zero;

        if (!leftController.isValid || !rightController.isValid)
            TryInitializeControllers();

        // Right controller (A/B, trigger, grip, stick)
        if (rightController.isValid)
        {
            rightController.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButton);
            rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButton);
            rightController.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed);
            rightController.TryGetFeatureValue(CommonUsages.gripButton, out gripPressed);
            rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out rightStick);
        }

        // Left controller (X/Y, trigger, grip, stick)
        if (leftController.isValid)
        {
            leftController.TryGetFeatureValue(CommonUsages.primaryButton, out xButton);
            leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out yButton);
            leftController.TryGetFeatureValue(CommonUsages.triggerButton, out leftTrigger);
            leftController.TryGetFeatureValue(CommonUsages.gripButton, out leftGrip);
            leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out leftStick);
        }

        Debug.Log(
            $"A:{primaryButton}  B:{secondaryButton}  X:{xButton}  Y:{yButton}  " +
            $"LeftTrigger:{leftTrigger}  RightTrigger:{triggerPressed}  " +
            $"LeftGrip:{leftGrip}  RightGrip:{gripPressed}  " +
            $"LeftStick:{leftStick}  RightStick:{rightStick}"
        );

    }
}
