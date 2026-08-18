using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam
{
    public class LevelScene : MonoBehaviour
    {
        public enum GameState
        {
            Playing,
            Paused,
            Ended,
        }

        public static LevelSO TargetLevel;

        [Header("Level Loading")]
        [SerializeField] private MapLevelLoader mapLevelLoader;
        [SerializeField] private LevelSO defaultLevel;

        private LebroManager lebroManager;

        private int lebrosSaved;
        private int lebrosSpawned;
        private float playtime;

        private GameState state;
        private LevelSO currentLevel;
        private LevelSO nextLevel;
        private LevelAttempt currentAttempt;

        private LebroStart currentLevelStart;

        public LevelSO CurrentLevel => currentLevel;
        public LevelSO NextLevel => nextLevel;

        public LebroStart CurrentLevelStart => currentLevelStart;

        public GameState State => state;
        public int LebrosSaved => lebrosSaved;
        public float Playtime => playtime;
        public LevelAttempt CurrentAttempt => currentAttempt;

        public Action<GameState> OnGameStateChanged;
        public Action<LevelScene> OnLevelLoaded;

        private void Awake()
        {
            lebroManager = GameContext.Instance.LebroManager;
        }

        private void Start()
        {
            if (TargetLevel == null)
            {
                LoadLevel(defaultLevel);
            }
            else
            {
                LoadLevel(TargetLevel);
                TargetLevel = null;
            }

            lebroManager.OnLebroSpawned += LebroManager_OnLebroSpawned;
            lebroManager.OnLebroRemoved += LebroManager_OnLebroRemoved;
        }

        private void Update()
        {
            UpdatePlaying();
        }

        private void UpdatePlaying()
        {
            if (currentLevel == null)
                return;

            if (state != GameState.Playing)
                return;

            playtime += Time.deltaTime;

            currentAttempt = new()
            {
                LebrosSaved = lebrosSaved,
                TimeSeconds = playtime,
            };

            currentAttempt.State = LevelAttempt.EvaluateAttemptState(currentAttempt, currentLevel);

            if (lebrosSpawned < currentLevel.TotalLebroCount)
                return;

            if (lebroManager.Lebros.Count != 0 && lebroManager.Lebros.Count >= (currentLevel.RequiredLebroSavedCount - lebrosSaved))
                return;

            ChangeGameState(GameState.Ended);

            // save attempt
            LevelData levelData = LevelDataStorage.LoadLevelData(currentLevel);
            levelData.Attempts.Add(currentAttempt);

            if ((int)currentAttempt.State > (int)levelData.State)
            {
                levelData.State = currentAttempt.State;
            }

            LevelDataStorage.SaveLevelData(levelData);
        }

        private void LebroManager_OnLebroSpawned(Lebro lebro)
        {
            lebrosSpawned++;
        }

        private void LebroManager_OnLebroRemoved(Lebro lebro)
        {
            lebrosSaved++;
        }

        private void ChangeGameState(GameState newState)
        {
            state = newState;
            OnGameStateChanged?.Invoke(state);
        }

        private void LoadLevel(LevelSO level)
        {
            if (currentLevel != null)
                throw new Exception($"A level is already loaded.");

            level.Validate();

            ChangeGameState(GameState.Playing);

            currentLevel = level;

            MainMenuScene.CurrentLevelList?.TryGetNextLevel(currentLevel, out nextLevel);

            lebrosSaved = 0;
            playtime = 0;

            GameObject world = mapLevelLoader.LoadLevel(currentLevel);
            currentLevelStart = world.GetComponentInChildren<LebroStart>();

            OnLevelLoaded?.Invoke(this);
        }

        public void RestartLevel()
        {
            TargetLevel = currentLevel;
            SceneManager.LoadScene((int)GameScene.LevelScene);
        }

        public void GoToNextLevel()
        {
            if (nextLevel == null)
                return;

            TargetLevel = nextLevel;
            SceneManager.LoadScene((int)GameScene.LevelScene);
        }

        public void Pause()
        {
            if (state != GameState.Playing)
                return;

            ChangeGameState(GameState.Paused);
        }

        public void Unpause()
        {
            ChangeGameState(GameState.Playing);
        }

        public void ExitToMainMenu()
        {
            MainMenuScene.TargetLevel = currentLevel;
            SceneManager.LoadScene((int)GameScene.MainMenuScene);
        }
    }
}
