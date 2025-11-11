using UnityEngine;

public class GarbageSpawner : MonoBehaviour
{
    public GameObject[] garbagePrefabs;  
    public float spawnInterval = 2f;     
    public float spawnHeight = 10f;      
    public Vector3 spawnArea;            

    void Start()
    {
        
        InvokeRepeating("SpawnGarbage", 0f, spawnInterval);
    }

    void SpawnGarbage()
    {
        int randomIndex = Random.Range(0, garbagePrefabs.Length);
        float randomX = Random.Range(-spawnArea.x, spawnArea.x);
        float randomZ = 0f; // ✅ Keep all garbage on Z = 0

        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, randomZ);

        Quaternion randomRotation = Quaternion.Euler(
            Random.Range(0f, 360f),
            Random.Range(0f, 360f),
            Random.Range(0f, 360f)
        );

        GameObject garbage = Instantiate(garbagePrefabs[randomIndex], spawnPosition, randomRotation);

        Rigidbody rb = garbage.GetComponent<Rigidbody>();
        if (rb == null) rb = garbage.AddComponent<Rigidbody>();
        rb.useGravity = true;

        Destroy(garbage, 15f);
    }


}
