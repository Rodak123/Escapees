using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameJam
{
    public class QuickPlayButton : MonoBehaviour
    {
        [SerializeField] private MainMenuScene mainMenuScene;

        [Header("UI")]
        [SerializeField] private GameObject container;
        [SerializeField] private Button button;
        [SerializeField] private Image coloredBackground;
        [SerializeField] private TMP_Text buttonText;

        [Header("Colors")]
        [SerializeField] private LevelStateColorsConfigSO colorsConfigSO;

        private void Awake()
        {
            button.onClick.AddListener(() =>
            {
                if (mainMenuScene.QuickPlayLevel.HasValue)
                {
                    mainMenuScene.QuickPlay();
                }
                else
                {
                    mainMenuScene.ToLevelSelect();
                }
            });
        }

        private void Update()
        {
            if (mainMenuScene.QuickPlayLevel.HasValue)
            {
                LevelListSO.Level level = mainMenuScene.QuickPlayLevel.Value;

                coloredBackground.color = colorsConfigSO.GetStateColor(level.LevelData.State, level.IsLocked);
                buttonText.text = $">{level.LevelSO.Area.AreaNumber}-{level.LevelSO.LevelNumber}";
            }
            else
            {
                coloredBackground.color = colorsConfigSO.GetStateColor(LevelState.Perfect, false);
                buttonText.text = $"DONE";
            }
        }
    }
}
