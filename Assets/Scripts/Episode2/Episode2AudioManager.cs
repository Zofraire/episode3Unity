using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Episode2
{
    /// <summary>
    /// Audio manager for Episode 2.
    /// Handles background music, voice lines, and sound effects.
    /// </summary>
    public class Episode2AudioManager : MonoBehaviour
    {
        public static Episode2AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource voiceSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Background Music")]
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] private float musicVolume = 0.5f;
        [SerializeField] private bool playMusicOnStart = true;

        [Header("Events")]
        [SerializeField] private UnityEvent OnVoiceStart;
        [SerializeField] private UnityEvent OnVoiceEnd;

        private Coroutine voiceCoroutine;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (playMusicOnStart && backgroundMusic != null)
            {
                PlayMusic(backgroundMusic);
            }
        }

        public void PlayMusic(AudioClip clip)
        {
            if (musicSource == null) return;

            musicSource.clip = clip;
            musicSource.volume = musicVolume;
            musicSource.loop = true;
            musicSource.Play();
        }

        public void StopMusic()
        {
            if (musicSource != null)
                musicSource.Stop();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
                musicSource.volume = musicVolume;
        }

        public void PlayVoiceLine(AudioClip clip)
        {
            if (voiceSource == null || clip == null) return;

            if (voiceCoroutine != null)
                StopCoroutine(voiceCoroutine);

            voiceCoroutine = StartCoroutine(PlayVoiceLineCoroutine(clip));
        }

        private IEnumerator PlayVoiceLineCoroutine(AudioClip clip)
        {
            // Lower music volume during voice
            float originalMusicVolume = musicSource != null ? musicSource.volume : 0;
            if (musicSource != null)
                musicSource.volume = originalMusicVolume * 0.3f;

            voiceSource.clip = clip;
            voiceSource.Play();
            OnVoiceStart?.Invoke();

            yield return new WaitForSeconds(clip.length);

            // Restore music volume
            if (musicSource != null)
                musicSource.volume = originalMusicVolume;

            OnVoiceEnd?.Invoke();
            voiceCoroutine = null;
        }

        public void PlaySFX(AudioClip clip)
        {
            if (sfxSource == null || clip == null) return;

            sfxSource.PlayOneShot(clip);
        }

        public void PlaySFX(AudioClip clip, float volume)
        {
            if (sfxSource == null || clip == null) return;

            sfxSource.PlayOneShot(clip, volume);
        }

        public bool IsVoicePlaying()
        {
            return voiceSource != null && voiceSource.isPlaying;
        }

        public void StopVoice()
        {
            if (voiceCoroutine != null)
            {
                StopCoroutine(voiceCoroutine);
                voiceCoroutine = null;
            }

            if (voiceSource != null)
                voiceSource.Stop();

            OnVoiceEnd?.Invoke();
        }

        public void StopAllAudio()
        {
            StopMusic();
            StopVoice();
            if (sfxSource != null)
                sfxSource.Stop();
        }
    }
}
