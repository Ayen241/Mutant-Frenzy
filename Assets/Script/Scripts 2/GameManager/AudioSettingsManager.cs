using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider gameMusicSlider; // NEW

    void Start()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float gameMusicVolume = PlayerPrefs.GetFloat("GameMusicVolume", 1f);

        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
        gameMusicSlider.value = gameMusicVolume;

        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
        SetGameMusicVolume(gameMusicVolume);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        gameMusicSlider.onValueChanged.AddListener(SetGameMusicVolume);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", LinearToDecibel(value));
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", LinearToDecibel(value));
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void SetGameMusicVolume(float value)
    {
        audioMixer.SetFloat("GameMusicVolume", LinearToDecibel(value));
        PlayerPrefs.SetFloat("GameMusicVolume", value);
    }

    private float LinearToDecibel(float value)
    {
        return (value <= 0.0001f) ? -80f : Mathf.Log10(value) * 20f;
    }
}
