using UnityEngine;

public class SharkSFX : MonoBehaviour
{
    public AudioClip biteSound;
    public AudioClip deathSound;

    public AudioSource audioSource;

    [Header("Bite Effect")]
    public GameObject biteEffectPrefab;
    public float effectDuration = 0.3f;

    public Transform biteEffectSpawnPoint; // Optional: where the effect should appear (e.g. near mouth)

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayBiteSound()
    {
        if (biteSound != null && audioSource != null)
            audioSource.PlayOneShot(biteSound);

        ShowBiteEffect();
    }

    public void PlayDeathSound()
    {
        if (deathSound != null && audioSource != null)
            audioSource.PlayOneShot(deathSound);
    }

    private void ShowBiteEffect()
    {
        if (biteEffectPrefab != null)
        {
            Vector3 spawnPos = (biteEffectSpawnPoint != null)
                ? biteEffectSpawnPoint.position
                : transform.position;

            GameObject effect = Instantiate(biteEffectPrefab, spawnPos, Quaternion.identity);
            Destroy(effect, effectDuration);
        }
    }
}
