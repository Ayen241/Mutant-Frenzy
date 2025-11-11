using UnityEngine;
using UnityEngine.UI;

public class AudioButton : MonoBehaviour
{
    public Sprite soundOnSprite;   // Sprite for sound ON
    public Sprite soundOffSprite;  // Sprite for sound OFF
    public Image buttonImage;      // Reference to the Image component

    private void Start()
    {
        UpdateIcon(); // Set correct icon on start
    }

    private void OnEnable()
    {
        UpdateIcon(); // Ensure it’s synced if UI is enabled mid-scene
    }

    public void OnAudioButtonClick()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleMute();
            UpdateIcon(); // Update after toggling
        }
        else
        {
            Debug.LogWarning("AudioManager not found!");
        }
    }

    private void UpdateIcon()
    {
        if (AudioManager.Instance == null || buttonImage == null) return;

        bool isMuted = AudioManager.Instance.IsMuted();
        buttonImage.sprite = isMuted ? soundOffSprite : soundOnSprite;
    }
}
