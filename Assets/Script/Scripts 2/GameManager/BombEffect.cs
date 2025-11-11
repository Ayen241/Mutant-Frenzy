using UnityEngine;

public class BombEffect : MonoBehaviour
{
    [Header("Explosion FX")]
    public GameObject explosionEffectPrefab;
    public float destroyDelay = 2f;

    [Header("Audio")]
    public AudioClip explosionSound;
    public AudioSource audioSource; // 🔧 Assign manually

    private void Awake()
    {
        // Optional fallback if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void TriggerExplosion(Vector3 position)
    {
        // Visual Effect
        if (explosionEffectPrefab != null)
        {
            GameObject fx = Instantiate(explosionEffectPrefab, position, Quaternion.identity);
            Destroy(fx, destroyDelay);
        }

        // Sound Effect
        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }
    }
}
