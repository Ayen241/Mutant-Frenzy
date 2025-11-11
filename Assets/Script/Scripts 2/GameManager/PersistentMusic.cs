using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentMusic : MonoBehaviour
{
    private static PersistentMusic currentInstance;

    [SerializeField] private string musicID = "default"; // Unique ID per scene
    [SerializeField] private float fadeDuration = 1.5f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (currentInstance != null)
        {
            if (currentInstance.musicID != this.musicID)
            {
                // Fade out the previous music and destroy it
                currentInstance.StartCoroutine(currentInstance.FadeOutAndDestroy());
            }
            else
            {
                // Same music already playing, destroy this one
                Destroy(gameObject);
                return;
            }
        }

        currentInstance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Optional: Handle scene-based fading if needed
    }

    public void FadeOutAndDestroyNow()
    {
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float startVolume = audioSource.volume;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime; // Use unscaled time
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
        Destroy(gameObject);
    }


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
