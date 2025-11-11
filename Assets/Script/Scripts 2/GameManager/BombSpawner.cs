using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    public GameObject[] bombPrefabs;       // Array of different bomb models
    public float spawnInterval = 4f;       // Interval between bomb spawns
    public float spawnHeight = 10f;        // Drop height
    public Vector3 spawnArea;              // Spawn range on X and Z
    public float bombLifetime = 10f;       // How long before bomb self-destructs

    void Start()
    {
        InvokeRepeating("SpawnBomb", 2f, spawnInterval); // Optional delay on start
    }

    void SpawnBomb()
    {
        int randomIndex = Random.Range(0, bombPrefabs.Length);
        float randomX = Random.Range(-spawnArea.x, spawnArea.x);
        float randomZ = Random.Range(-spawnArea.z, spawnArea.z);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, randomZ);

        Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        GameObject bomb = Instantiate(bombPrefabs[randomIndex], spawnPosition, randomRotation);

        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        if (rb == null) rb = bomb.AddComponent<Rigidbody>();
        rb.useGravity = true;

        // Auto-destroy after X seconds
        Destroy(bomb, bombLifetime);
    }
}
