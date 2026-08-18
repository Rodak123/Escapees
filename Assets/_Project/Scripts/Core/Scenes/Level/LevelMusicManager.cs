using UnityEngine;

namespace GameJam
{
    public class LevelMusicManager : MonoBehaviour
    {
        [SerializeField] private LevelScene levelScene;

        [Header("Fade out")]
        [SerializeField] private float fadeOutDuration = 3;

        [Header("Muffle")]
        [SerializeField] private float muffledFreq = 300;

        private MusicManager musicManager;

        private MusicSO CurrentLevelMusic => levelScene.CurrentLevel?.Music;

        private void Awake()
        {
            musicManager = MusicManager.Instance;

            levelScene.OnLevelLoaded += LevelScene_OnLevelLoaded;
            levelScene.OnGameStateChanged += LevelScene_OnGameStateChanged;
            musicManager.OnCurrentMusicEnded += MusicManager_OnCurrentMusicEnded;
        }

        private void OnDisable()
        {
            SetMuffledFilter(false);
        }

        private void SetMuffledFilter(bool enabled)
        {
            if (AudioSettingsManager.HasInstance)
                AudioSettingsManager.Instance.SetAudioMixerLowpass(enabled ? muffledFreq : float.MaxValue, AudioSettingsManager.AudioMixerCategory.Music);
        }

        private void LevelScene_OnLevelLoaded(LevelScene leveLScene)
        {
            SFXManager.Instance.StopAllSFX();
            SetMuffledFilter(false);

            if (CurrentLevelMusic == null)
            {
                musicManager.StopMusic(0);
                return;
            }

            if (musicManager.CurrentMusic != CurrentLevelMusic)
            {
                // replace playing music with current music
                musicManager.PlayMusic(CurrentLevelMusic);
            }
        }

        private void LevelScene_OnGameStateChanged(LevelScene.GameState state)
        {
            SetMuffledFilter(state == LevelScene.GameState.Paused);

            if (state == LevelScene.GameState.Ended)
            {
                musicManager.StopMusic(fadeOutDuration);
            }
        }

        private void MusicManager_OnCurrentMusicEnded(MusicSO music)
        {
            if (music != CurrentLevelMusic)
                return;

            // loop current level music
            if (levelScene.State != LevelScene.GameState.Ended)
                musicManager.PlayMusic(music);
        }
    }
}
