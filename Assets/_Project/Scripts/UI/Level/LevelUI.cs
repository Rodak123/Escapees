using UnityEngine;
using TMPro;

namespace GameJam
{
    public class LevelUI : MonoBehaviour
    {
        [SerializeField] private LevelScene levelScene;

        [Header("UI")]
        [SerializeField] private TMP_Text infoText;
        [SerializeField] private LevelStateColorsConfigSO colorsConfigSO;

        private void Update()
        {
            string completeCount = $"{levelScene.CurrentLevel.RequiredLebroSavedCount}".WrapInColor(colorsConfigSO.GetStateColor(LevelState.Complete, false));
            string perfectCount = $"{levelScene.CurrentLevel.TotalLebroCount}".WrapInColor(colorsConfigSO.GetStateColor(LevelState.Perfect, false));

            string currentSaveCount = $"{levelScene.LebrosSaved}";

            if (levelScene.CurrentAttempt != null)
            {
                currentSaveCount = currentSaveCount.WrapInColor(colorsConfigSO.GetStateColor(levelScene.CurrentAttempt.State, false));
            }

            infoText.text = $"{currentSaveCount}/{completeCount}/{perfectCount}";
        }
    }
}
