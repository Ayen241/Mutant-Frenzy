using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource audioSource;
    private bool isMuted;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found!");
            return;
        }

        audioSource.loop = true;

        // ✅ Load and apply saved mute state
        isMuted = PlayerPrefs.GetInt("musicMuted", 0) == 1;
        audioSource.mute = isMuted;

        audioSource.Play();
    }

    public void ToggleMute()
    {
        // ✅ Flip the state
        isMuted = !isMuted;

        // ✅ Apply to audio
        audioSource.mute = isMuted;

        // ✅ Save the state
        PlayerPrefs.SetInt("musicMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool IsMuted()
    {
        return isMuted;
    }
}
