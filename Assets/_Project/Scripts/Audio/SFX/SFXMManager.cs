using Rodak.Utils.Singleton;
using UnityEngine;

namespace GameJam
{
    [DefaultExecutionOrder(-50)]
    public class SFXManager : SingletonMonoBehaviour<SFXManager>
    {
        [SerializeField] private AudioSource sfxAudioSource;

        public void StopAllSFX()
        {
            sfxAudioSource.Stop();
        }

        public void PlaySFX(SoundEffect soundEffect, float volumeScale, AudioSource audioSource)
        {
            PlayClip(soundEffect.PickRandom(), volumeScale * soundEffect.VolumeScale, audioSource);
        }

        public void PlaySFX(SoundEffect soundEffect, float volumeScale)
        {
            PlaySFX(soundEffect, volumeScale, sfxAudioSource);
        }

        public void PlayClip(AudioClip clip, float volumeScale, AudioSource audioSource)
        {
            if (clip == null) return;
            audioSource.PlayOneShot(clip, volumeScale);
        }

        public void PlayClip(AudioClip clip, float volumeScale)
        {
            PlayClip(clip, volumeScale, sfxAudioSource);
        }

        public void PlayClip(AudioClip clip)
        {
            PlayClip(clip, 1, sfxAudioSource);
        }
    }
}