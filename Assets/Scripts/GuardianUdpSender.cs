using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.XR;

public class GuardianUdpSender : MonoBehaviour
{
    [Header("Pi Network")]
    [Tooltip("If your Pi is running as an AP, its IP is often 192.168.4.1. Change if needed.")]
    public string piIp = "192.168.4.1";
    public int piPort = 33333;

    [Header("Send")]
    [Range(1, 120)] public int sendRateHz = 60;

    private InputDevice left, right;
    private UdpClient udp;
    private float sendInterval;
    private float nextSend;

    // Modes
    private int mode = 1; // 1=Driving, 2=Arm
    private bool armRightStickElbow = false; // false=Shoulder, true=Elbow
    private bool gripperClosed = false;

    // Edge detection
    private bool prevA, prevB, prevRightTriggerBtn;

    void Start()
    {
        sendInterval = 1f / Mathf.Max(1, sendRateHz);
        TryInitControllers();

        udp = new UdpClient();
        udp.Connect(piIp, piPort);
    }

    void OnDestroy()
    {
        try { udp?.Close(); } catch { }
        InputDevices.deviceConnected -= OnDeviceConnected;
        InputDevices.deviceDisconnected -= OnDeviceDisconnected;
    }

    void OnEnable()
    {
        InputDevices.deviceConnected += OnDeviceConnected;
        InputDevices.deviceDisconnected += OnDeviceDisconnected;
    }

    void OnDeviceConnected(InputDevice dev) { TryInitControllers(); }
    void OnDeviceDisconnected(InputDevice dev) { TryInitControllers(); }

    void TryInitControllers()
    {
        var lefts = new System.Collections.Generic.List<InputDevice>();
        var rights = new System.Collections.Generic.List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, lefts);
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rights);
        if (lefts.Count > 0) left = lefts[0];
        if (rights.Count > 0) right = rights[0];
    }

    void Update()
    {
        if (!left.isValid || !right.isValid) TryInitControllers();

        // Read buttons / axes
        bool a = false, b = false, x = false, y = false;
        bool leftGrip = false, rightGrip = false, leftTrigBtn = false, rightTrigBtn = false;
        Vector2 leftStick = Vector2.zero, rightStick = Vector2.zero;

        if (right.isValid)
        {
            right.TryGetFeatureValue(CommonUsages.primaryButton, out a);      // A
            right.TryGetFeatureValue(CommonUsages.secondaryButton, out b);    // B
            right.TryGetFeatureValue(CommonUsages.gripButton, out rightGrip);
            right.TryGetFeatureValue(CommonUsages.triggerButton, out rightTrigBtn);
            right.TryGetFeatureValue(CommonUsages.primary2DAxis, out rightStick);
        }
        if (left.isValid)
        {
            left.TryGetFeatureValue(CommonUsages.primaryButton, out x);       // X
            left.TryGetFeatureValue(CommonUsages.secondaryButton, out y);     // Y
            left.TryGetFeatureValue(CommonUsages.gripButton, out leftGrip);
            left.TryGetFeatureValue(CommonUsages.triggerButton, out leftTrigBtn);
            left.TryGetFeatureValue(CommonUsages.primary2DAxis, out leftStick);
        }

        // Edge toggles
        if (a && !prevA) mode = (mode == 1) ? 2 : 1;                    // A: mode switch
        if (b && !prevB && mode == 2) armRightStickElbow = !armRightStickElbow; // B: shoulder<->elbow
        if (rightTrigBtn && !prevRightTriggerBtn && mode == 2)           // Right trigger click
            gripperClosed = !gripperClosed;

        prevA = a; prevB = b; prevRightTriggerBtn = rightTrigBtn;

        // Send at fixed rate
        if (Time.time >= nextSend)
        {
            nextSend = Time.time + sendInterval;

            var payload = new
            {
                ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                mode = mode,                         // 1=drive, 2=arm
                armElbow = armRightStickElbow,       // in mode 2, right stick targets elbow if true
                gripperClosed = gripperClosed,       // toggled by right trigger click
                buttons = new
                {
                    A = a,
                    B = b,
                    X = x,
                    Y = y,
                    LeftGrip = leftGrip,
                    RightGrip = rightGrip,
                    LeftTrigger = leftTrigBtn,
                    RightTrigger = rightTrigBtn
                },
                sticks = new
                {
                    leftX = leftStick.x,
                    leftY = leftStick.y,
                    rightX = rightStick.x,
                    rightY = rightStick.y
                }
            };
            Debug.Log(
            $"A:{a}  B:{b}  X:{x}  Y:{y}  " +
            $"LeftTrigger:{leftTrigBtn}  RightTrigger:{rightTrigBtn}  " +
            $"LeftGrip:{leftGrip}  RightGrip:{rightGrip}  " +
            $"LeftStick:{leftStick}  RightStick:{rightStick}"
        );
            string json = JsonUtility.ToJson(payload);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            try { udp.Send(bytes, bytes.Length); } catch { }
        }
    }
}
