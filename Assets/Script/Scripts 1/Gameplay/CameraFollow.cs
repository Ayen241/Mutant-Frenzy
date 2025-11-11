using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform shark;           // Reference to the shark's transform
    public float followSpeed = 5f;    // Speed at which the camera follows the shark
    public Vector3 offset;           // Offset from the shark, to maintain a set distance

    void Start()
    {
        // If no shark is assigned in the Inspector, try to find the shark in the scene
        if (shark == null)
        {
            shark = GameObject.FindWithTag("Shark").transform;
        }
    }

    void Update()
    {
        if (shark != null)
        {
            // Camera follows the shark, keeping the defined offset
            Vector3 desiredPosition = shark.position + offset;

            // Smoothly move the camera towards the desired position
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        }
    }
}
