using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Runtime.InteropServices;
using TMPro;

namespace GameJam
{
    public class LevelSharer : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void CopyToClipboard(string str, string alertText);

        [SerializeField] private MainMenuScene mainMenuScene;

        [Header("UI")]
        [SerializeField] private GameObject levelStatsContainer;

        [Header("Alert Text UI")]
        [SerializeField] private TMP_Text alertText;
        [SerializeField] private float alertTextDecay = 1;

        private readonly List<LevelListSO.Level> sharableLevels = new();

        public string ShareText { get; private set; }

        private float alertTextAlpha;

        private void Start()
        {
            levelStatsContainer.SetActive(false);
            UpdateShareText();
            alertTextAlpha = 0;
        }

        private void Update()
        {
            sharableLevels.Clear();
            sharableLevels.AddRange(GetSharableLevels());

            levelStatsContainer.SetActive(sharableLevels.Count > 0);

            if (alertTextAlpha > 0)
            {
                alertTextAlpha = Mathf.Max(0, alertTextAlpha - alertTextDecay * Time.deltaTime);
            }
            alertText.color = new(alertText.color.r, alertText.color.g, alertText.color.b, alertTextAlpha);
        }

        private IEnumerable<LevelListSO.Level> GetSharableLevels()
        {
            foreach (LevelListSO.Level level in mainMenuScene.LevelList.GetLevelStates())
            {
                yield return level;
            }
        }

        private string GetLevelStateEmoji(LevelState state)
        {
            return state switch
            {
                LevelState.Unplayed => "⬜",
                LevelState.Failed => "🟥",
                LevelState.Complete => "🟩",
                LevelState.Perfect => "🟨",
                _ => "⬛",
            };
        }

        private void UpdateShareText()
        {
            if (sharableLevels.Count == 0)
            {
                ShareText = string.Empty;
                return;
            }

            int maxLevelNameLength = 0;
            foreach (LevelSO level in mainMenuScene.LevelList.Levels)
            {
                maxLevelNameLength = Mathf.Max(maxLevelNameLength, level.ToString().Length);
            }

            StringBuilder shareTextBuilder = new();
            foreach (LevelListSO.Level level in sharableLevels)
            {
                if (level.IsLocked)
                {
                    string secretLevelString = level.LevelSO.GetLevelString(false).PadRight(maxLevelNameLength, ' ');
                    shareTextBuilder.AppendLine($">{secretLevelString} ???");
                    continue;
                }

                string bestTimeString = string.Empty;
                if (level.LevelData.TryGetBestTimeSeconds(out float bestTimeSeconds, LevelState.Complete))
                {
                    bestTimeString = $" with best time of {StringFormatting.FormatTime(bestTimeSeconds)}";
                }

                string attemptsString = string.Empty;
                if (level.LevelData.Attempts.Count > 0)
                {
                    attemptsString = $" in {level.LevelData.Attempts.Count} attempts";
                }

                string stateEmoji = GetLevelStateEmoji(level.LevelData.State);
                string levelString = level.LevelSO.GetLevelString(true).PadRight(maxLevelNameLength, ' ');

                shareTextBuilder.AppendLine($">{levelString} {stateEmoji} {level.LevelData.State}{bestTimeString}{attemptsString}");
            }
            ShareText = shareTextBuilder.ToString();
        }

        public void ShareLevels()
        {
            UpdateShareText();

            if (string.IsNullOrEmpty(ShareText)) return;

            alertTextAlpha = 1;

#if UNITY_WEBGL && !UNITY_EDITOR
            CopyToClipboard(ShareText, "Levels copied to clipboard! I'll love to see your results in the comments below :D");
#else
            GUIUtility.systemCopyBuffer = ShareText;
#endif
        }
    }
}