using UnityEngine;

namespace GameJam
{
    public class IntroMusicManager : MonoBehaviour
    {
        [SerializeField] private MusicSO introMusic;

        private void Start()
        {
            SFXManager.Instance.StopAllSFX();
            AudioSettingsManager.Instance.ClearAudioMixerLowpass();
            MusicManager.Instance.PlayMusic(introMusic);
        }
    }
}
