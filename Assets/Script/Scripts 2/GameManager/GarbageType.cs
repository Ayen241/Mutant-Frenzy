using UnityEngine;

public class GarbageType : MonoBehaviour
{
    public enum GarbageSize { Small, Medium, Big }
    public GarbageSize size;
    public int points = 10; // Default points for small

    // You can set size and points per prefab in Inspector
}
