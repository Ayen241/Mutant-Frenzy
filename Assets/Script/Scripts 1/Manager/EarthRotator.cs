using UnityEngine;

public class EarthRotator : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public float maxVerticalAngle = 80f; // Limit vertical rotation (latitude)

    private Vector3 lastMousePosition;
    private float verticalRotation = 0f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            lastMousePosition = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            float rotX = delta.x * rotationSpeed * Time.deltaTime; // Horizontal rotation (left/right)
            float rotY = -delta.y * rotationSpeed * Time.deltaTime; // Vertical rotation (up/down)

            // Limit vertical rotation to avoid flipping
            verticalRotation -= rotY;
            verticalRotation = Mathf.Clamp(verticalRotation, -maxVerticalAngle, maxVerticalAngle);

            // Rotate around the Y axis (left and right)
            transform.Rotate(Vector3.up, -rotX, Space.World);

            // Rotate around the X axis (up and down) with clamped vertical rotation
            transform.localRotation = Quaternion.Euler(verticalRotation, transform.eulerAngles.y, 0);

            lastMousePosition = Input.mousePosition;
        }
    }
}
