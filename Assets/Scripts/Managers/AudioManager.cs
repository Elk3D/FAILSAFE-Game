using UnityEngine;

namespace FAILSAFE.Managers
{
    /// <summary>
    /// Central audio manager. Routes SFX, music, and dialogue through
    /// dedicated AudioSources with independent volume controls.
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource ambienceSource;
        [SerializeField] private AudioSource dialogueSource;

        [Header("Volume Settings")]
        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float musicVolume = 0.8f;
        [Range(0f, 1f)] public float sfxVolume = 1f;
        [Range(0f, 1f)] public float ambienceVolume = 0.6f;
        [Range(0f, 1f)] public float dialogueVolume = 1f;

        public void PlaySFX(AudioClip clip, float pitch = 1f)
        {
            if (clip == null) return;
            sfxSource.pitch = pitch;
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip == null) return;
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.volume = musicVolume * masterVolume;
            musicSource.Play();
        }

        public void PlayAmbience(AudioClip clip, bool loop = true)
        {
            if (clip == null) return;
            ambienceSource.clip = clip;
            ambienceSource.loop = loop;
            ambienceSource.volume = ambienceVolume * masterVolume;
            ambienceSource.Play();
        }

        public void PlayDialogue(AudioClip clip)
        {
            if (clip == null) return;
            dialogueSource.clip = clip;
            dialogueSource.volume = dialogueVolume * masterVolume;
            dialogueSource.Play();
        }

        public void StopAll()
        {
            musicSource.Stop();
            sfxSource.Stop();
            ambienceSource.Stop();
            dialogueSource.Stop();
        }

        public void ApplyVolumeSettings()
        {
            musicSource.volume = musicVolume * masterVolume;
            ambienceSource.volume = ambienceVolume * masterVolume;
        }
    }
}
