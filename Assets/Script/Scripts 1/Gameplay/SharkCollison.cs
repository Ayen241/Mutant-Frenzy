using UnityEngine;

public class SharkCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Garbage"))
        {
            // Destroy the garbage object when it collides with the shark
            Destroy(other.gameObject);
        }
    }
}
