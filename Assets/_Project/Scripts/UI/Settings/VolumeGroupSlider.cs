using UnityEngine;
using UnityEngine.UI;

namespace GameJam
{
    public class VolumeGroupSlider : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Slider slider;

        [Header("Audio")]
        [SerializeField] private AudioSettingsManager.AudioMixerCategory mixerCategory = AudioSettingsManager.AudioMixerCategory.Master;

        private void Start()
        {
            slider.value = AudioSettingsManager.Instance.GetVolume(mixerCategory);
            slider.onValueChanged.AddListener(Slider_OnValueChanged);

            AudioSettingsManager.Instance.OnSettingsChanged += AudioSettingsManager_OnSettingsChanged;
        }

        private void AudioSettingsManager_OnSettingsChanged()
        {
            slider.value = AudioSettingsManager.Instance.GetVolume(mixerCategory);
        }

        private void Slider_OnValueChanged(float value)
        {
            AudioSettingsManager.Instance.SetVolume(value, mixerCategory);
            AudioSettingsManager.Instance.ApplyAllSettings();
        }
    }
}