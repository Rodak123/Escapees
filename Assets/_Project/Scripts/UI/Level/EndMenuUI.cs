using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameJam
{
    public class EndMenuUI : MonoBehaviour
    {
        [SerializeField] private LevelScene levelScene;

        [Header("UI")]
        [SerializeField] private GameObject endMenuContainer;

        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text stateText;
        [SerializeField] private TMP_Text nextLevelText;

        [SerializeField] private Button restartButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button exitButton;

        [SerializeField] private LevelStateColorsConfigSO colorsConfigSO;

        private void Awake()
        {
            levelScene.OnGameStateChanged += LevelScene_OnGameStateChanged;

            restartButton.onClick.AddListener(() => levelScene.RestartLevel());
            continueButton.onClick.AddListener(() => levelScene.GoToNextLevel());
            exitButton.onClick.AddListener(() => levelScene.ExitToMainMenu());
        }

        private void Update()
        {
            if (levelScene.CurrentAttempt == null)
                return;

            timeText.text = StringFormatting.FormatTime(levelScene.CurrentAttempt.TimeSeconds);

            LevelState currentState = levelScene.CurrentAttempt.State;
            stateText.text = currentState.ToString().ToUpper().WrapInColor(colorsConfigSO.GetStateColor(currentState, false));

            if (levelScene.CurrentAttempt.IsCompleted)
            {
                LevelSO nextLevel = levelScene.NextLevel;
                if (levelScene.NextLevel != null)
                {
                    continueButton.gameObject.SetActive(true);
                    nextLevelText.text = $">{nextLevel.Area.AreaNumber}-{nextLevel.LevelNumber}";
                }
                else
                {
                    continueButton.gameObject.SetActive(false);
                }
            }
            else
            {
                continueButton.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            endMenuContainer.SetActive(levelScene.State == LevelScene.GameState.Ended);
        }

        private void LevelScene_OnGameStateChanged(LevelScene.GameState state)
        {
            endMenuContainer.SetActive(state == LevelScene.GameState.Ended);
        }
    }
}
