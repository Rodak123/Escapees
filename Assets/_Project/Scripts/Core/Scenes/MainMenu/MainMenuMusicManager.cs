using UnityEngine;

namespace GameJam
{
    public class MainMenuMusicManager : MonoBehaviour
    {
        [SerializeField] private MusicSO mainMenuMusic;

        private void Start()
        {
            SFXManager.Instance.StopAllSFX();
            AudioSettingsManager.Instance.ClearAudioMixerLowpass();
            MusicManager.Instance.PlayMusicOrContinue(mainMenuMusic);
        }
    }
}
