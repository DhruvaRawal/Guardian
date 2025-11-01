using UnityEngine;

public class WheelSpin : MonoBehaviour
{
    public Transform carBody;
    public float rotationMultiplier = 10f;

    private Vector3 lastPosition;

    void Start()
    {
        if (carBody == null)
        {
            Debug.LogError("WheelSpin: Car body not assigned!");
            enabled = false;
            return;
        }

        lastPosition = carBody.position;
    }

    void Update()
    {
        float distance = (carBody.position - lastPosition).magnitude;
        float rotationAmount = distance * rotationMultiplier;
        transform.Rotate(Vector3.right, rotationAmount, Space.Self);
        lastPosition = carBody.position;
    }
}
