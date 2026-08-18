using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

namespace GameJam
{
    public class LevelSelectButtonUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button button;
        [SerializeField] private Image coloredBackground;
        [SerializeField] private TMP_Text levelNumberText;

        [Header("Colors")]
        [SerializeField] private LevelStateColorsConfigSO colorsConfigSO;

        private LevelSO levelSO;
        private bool isLocked;

        public LevelSO LevelSO => levelSO;

        public event Action<LevelSelectButtonUI> OnClicked;


        private void Awake()
        {
            button.onClick.AddListener(() =>
            {
                if (isLocked)
                    return;
                OnClicked?.Invoke(this);
            });
        }

        public void SetLevel(LevelListSO.Level level)
        {
            levelSO = level.LevelSO;
            isLocked = level.IsLocked;
            button.interactable = !level.IsLocked;
            coloredBackground.color = colorsConfigSO.GetStateColor(level.LevelData.State, level.IsLocked);
            levelNumberText.text = $"{levelSO.LevelNumber}";
        }
    }
}
