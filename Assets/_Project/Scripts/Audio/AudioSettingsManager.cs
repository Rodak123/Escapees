using System;
using System.Collections.Generic;
using Rodak.Utils.Singleton;
using UnityEngine;
using UnityEngine.Audio;

namespace GameJam
{
    public class AudioSettingsManager : SingletonMonoBehaviour<AudioSettingsManager>
    {
        [Serializable]
        public enum AudioMixerCategory
        {
            Master,
            SFX,
            Music,
        }

        [Serializable]
        public struct AudioMixerCategoryVolume
        {
            public AudioMixerCategory Category;
            public float Volume;
        }

        private const string AudioPrefix = "Audio";
        private const string VolumeSuffix = "Volume";
        private const string LowPassSuffix = "LowpassCutofffreq";

        [SerializeField] private AudioMixer audioMixer;

        [Header("Defaults")]
        [SerializeField] private AudioMixerCategoryVolume[] defaultMixerCategoryVolumes;

        [Header("Volume Scalers")]
        [SerializeField] private AudioMixerCategoryVolume[] globalCategoryVolumeScalers;

        private readonly Dictionary<AudioMixerCategory, float> categoryVolumes = new();
        public event Action OnSettingsChanged;

        protected override void Awake()
        {
            base.Awake();

            foreach (AudioMixerCategory category in (AudioMixerCategory[])Enum.GetValues(typeof(AudioMixerCategory)))
            {
                categoryVolumes.Add(category, 1f);
            }

            foreach (AudioMixerCategoryVolume categoryVolume in defaultMixerCategoryVolumes)
            {
                categoryVolumes[categoryVolume.Category] = categoryVolume.Volume;
            }

            LoadSettings();
        }

        private void Start()
        {
            ApplyAllSettings();
        }

        private void UpdateMixers()
        {
            foreach (AudioMixerCategory category in new List<AudioMixerCategory>(categoryVolumes.Keys))
            {
                SetAudioMixerVolume(categoryVolumes[category], category);
            }
        }

        // converts from % value to DB with dB = 20 * log_10(value)
        private void SetAudioMixerVolume(float value, AudioMixerCategory category)
        {
            foreach (AudioMixerCategoryVolume categoryVolume in globalCategoryVolumeScalers)
            {
                if (category != categoryVolume.Category) continue;
                value *= categoryVolume.Volume;
            }

            float dbValue = Mathf.Log10(value) * 20f;
            audioMixer.SetFloat($"{category}{VolumeSuffix}", dbValue);
        }

        private void SaveSettings()
        {
            foreach (AudioMixerCategory category in new List<AudioMixerCategory>(categoryVolumes.Keys))
            {
                PlayerPrefs.SetFloat($"{AudioPrefix}{category}", categoryVolumes[category]);
            }
            PlayerPrefs.Save();
        }

        private void LoadSettings()
        {
            foreach (AudioMixerCategory category in new List<AudioMixerCategory>(categoryVolumes.Keys))
            {
                categoryVolumes[category] = PlayerPrefs.GetFloat($"{AudioPrefix}{category}", categoryVolumes[category]);
            }
        }

        public float GetVolume(AudioMixerCategory category)
        {
            return categoryVolumes[category];
        }

        public void SetVolume(float volume, AudioMixerCategory category)
        {
            categoryVolumes[category] = Mathf.Clamp(volume, 0.0001f, 1f);
        }

        public void ApplyAllSettings()
        {
            SaveSettings();
            UpdateMixers();
            OnSettingsChanged?.Invoke();
        }

        public void ResetAllSettings()
        {
            foreach (AudioMixerCategory category in (AudioMixerCategory[])Enum.GetValues(typeof(AudioMixerCategory)))
            {
                categoryVolumes[category] = 1f;
            }

            foreach (AudioMixerCategoryVolume categoryVolume in defaultMixerCategoryVolumes)
            {
                categoryVolumes[categoryVolume.Category] = categoryVolume.Volume;
            }

            ApplyAllSettings();
        }

        public void ClearAudioMixerLowpass()
        {
            foreach (AudioMixerCategory category in (AudioMixerCategory[])Enum.GetValues(typeof(AudioMixerCategory)))
            {
                SetAudioMixerLowpass(float.MaxValue, category);
            }
        }

        public void SetAudioMixerLowpass(float freq, AudioMixerCategory category)
        {
            audioMixer.SetFloat($"{category}{LowPassSuffix}", freq);
        }
    }
}