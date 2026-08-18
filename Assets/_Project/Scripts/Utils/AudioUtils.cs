using System;
using System.Collections;
using UnityEngine;

namespace GameJam
{
    public static class AudioUtils
    {
        // https://discussions.unity.com/t/fade-out-audio-source/585912
        public static IEnumerator FadeOut(AudioSource audioSource, float fadeTime, Action onFinished = null)
        {
            fadeTime = Mathf.Max(0, fadeTime);

            float startVolume = audioSource.volume;

            while (fadeTime != 0 && audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / fadeTime;

                yield return null;
            }

            audioSource.Stop();
            audioSource.volume = startVolume;

            onFinished?.Invoke();
        }

        public static IEnumerator FadeIn(AudioSource audioSource, float fadeTime, float targetVolume = 1, Action onFinished = null)
        {
            fadeTime = Mathf.Max(0, fadeTime);

            float startVolume = audioSource.volume;
            audioSource.Play();

            while (fadeTime != 0 && audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / fadeTime;

                yield return null;
            }

            audioSource.volume = targetVolume;

            onFinished?.Invoke();
        }
    }
}
