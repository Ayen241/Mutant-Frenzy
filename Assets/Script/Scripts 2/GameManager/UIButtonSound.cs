using UnityEngine;

public class UIButtonSound : MonoBehaviour
{
    public enum ButtonSoundType { Click, OpenPanel }

    [Header("Audio Clips")]
    public AudioClip clickSound;
    public AudioClip openPanelSound;

    [Header("Optional Direct Assignment")]
    public AudioSource audioSource; // drag this in manually

    void Awake()
    {
        if (audioSource == null)
            audioSource = FindObjectOfType<AudioSource>();
    }

    public void PlayClick()
    {
        PlaySound(ButtonSoundType.Click);
    }

    public void PlayOpenPanel()
    {
        PlaySound(ButtonSoundType.OpenPanel);
    }

    private void PlaySound(ButtonSoundType type)
    {
        if (audioSource == null) return;

        switch (type)
        {
            case ButtonSoundType.Click:
                if (clickSound != null) audioSource.PlayOneShot(clickSound);
                break;

            case ButtonSoundType.OpenPanel:
                if (openPanelSound != null) audioSource.PlayOneShot(openPanelSound);
                break;
        }
    }
}
