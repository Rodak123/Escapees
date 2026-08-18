using UnityEngine;
using TMPro;

namespace GameJam
{
    public class TotalLevelTimeText : MonoBehaviour
    {
        [SerializeField] private MainMenuScene mainMenu;

        [Header("UI")]
        [SerializeField] private TMP_Text text;

        private void Awake()
        {
            float totalBestTimeSeconds = 0;
            foreach (LevelListSO.Level level in mainMenu.LevelList.GetLevelStates())
            {
                if (!level.LevelData.TryGetBestTimeSeconds(out float bestTimeSeconds, LevelState.Complete))
                    continue;
                totalBestTimeSeconds += bestTimeSeconds;
            }

            text.text = StringFormatting.FormatTime(totalBestTimeSeconds);
        }
    }
}
