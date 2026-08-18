using UnityEngine;
using TMPro;

namespace GameJam
{
    public class LevelDetailsUIl : MonoBehaviour
    {
        [SerializeField] private MainMenuScene mainMenuScene;

        [Header("UI")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text levelNameText;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text bestTimeText;


        [Header("Colors")]
        [SerializeField] private LevelStateColorsConfigSO colorsConfigSO;

        private void Update()
        {
            if (mainMenuScene.SelectedLevel != null)
            {
                UpdateUI(mainMenuScene.SelectedLevel);
            }
        }

        private void UpdateUI(LevelSO level)
        {
            LevelData levelData = LevelDataStorage.LoadLevelData(level);

            titleText.text = $"#{level.Area.AreaNumber}-{level.LevelNumber} {level.Area.AreaName}";
            levelNameText.text = level.LevelName;

            statusText.text = levelData.State.ToString().ToUpper().WrapInColor(colorsConfigSO.GetStateColor(levelData.State, false));

            bool hasBestTime = levelData.TryGetBestTimeSeconds(out float bestTime, LevelState.Complete);
            bestTimeText.text = hasBestTime ? StringFormatting.FormatTime(bestTime) : "N/A";
        }
    }
}
