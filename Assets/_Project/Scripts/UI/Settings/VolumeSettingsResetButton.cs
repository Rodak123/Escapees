using UnityEngine;

namespace GameJam
{
    public class VolumeSettingsResetButton : MonoBehaviour
    {
        public void ResetVolumeSettings()
        {
            AudioSettingsManager.Instance.ResetAllSettings();
        }
    }
}