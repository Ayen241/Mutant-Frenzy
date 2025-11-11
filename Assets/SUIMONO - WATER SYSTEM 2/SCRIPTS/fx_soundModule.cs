using UnityEngine;
using UnityEngine.Audio; // ✅ Add this
using System.Collections;

namespace Suimono.Core
{
    public class fx_soundModule : MonoBehaviour
    {
        public int maxSounds = 10;
        public AudioClip abovewaterSound;
        public AudioClip underwaterSound;
        public AudioClip[] defaultSplashSound;

        [Header("Mixer")]
        public AudioMixerGroup waterMixerGroup; // ✅ Assign this in Inspector

        private AudioSource audioSrc;

        void Start()
        {
            audioSrc = GetComponent<AudioSource>();
            if (audioSrc == null)
            {
                audioSrc = gameObject.AddComponent<AudioSource>();
            }

            if (waterMixerGroup != null)
            {
                audioSrc.outputAudioMixerGroup = waterMixerGroup; // ✅ Route through mixer
            }
        }

        // Optional: Example of playing a sound
        public void PlayAboveWaterSound()
        {
            if (abovewaterSound != null)
            {
                audioSrc.PlayOneShot(abovewaterSound);
            }
        }
    }
}
