using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameJam
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private LevelScene levelScene;

        [Header("UI")]
        [SerializeField] private GameObject pauseMenuContainer;

        [Space]
        [SerializeField] private TMP_Text statsText;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button settingsButton;

        [Space]
        [SerializeField] private SettingsMenuUI settingsMenu;

        private void Awake()
        {
            levelScene.OnGameStateChanged += LevelScene_OnGameStateChanged;

            continueButton.onClick.AddListener(() => levelScene.Unpause());
            restartButton.onClick.AddListener(() => levelScene.RestartLevel());
            exitButton.onClick.AddListener(() => levelScene.ExitToMainMenu());
            settingsButton.onClick.AddListener(() => settingsMenu.SetSettingsVisibility(true));

            settingsMenu.SetSettingsVisibility(false);
        }

        private void OnDisable()
        {
            settingsMenu.SetSettingsVisibility(false);
        }

        private void Update()
        {
            statsText.text = StringFormatting.FormatTime(levelScene.Playtime);
        }

        private void Start()
        {
            pauseMenuContainer.SetActive(levelScene.State == LevelScene.GameState.Paused);
        }

        private void LevelScene_OnGameStateChanged(LevelScene.GameState state)
        {
            bool isPaused = state == LevelScene.GameState.Paused;
            pauseMenuContainer.SetActive(isPaused);
            if (!isPaused)
            {
                settingsMenu.SetSettingsVisibility(false);
            }
        }
    }
}
