using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    public class LevelSelectUI : MonoBehaviour
    {
        [SerializeField] private MainMenuScene mainMenuScene;

        [Header("UI")]
        [SerializeField] private LevelSelectButtonUI buttonPrefab;
        [SerializeField] private Transform levelButtonContainer;

        private readonly Dictionary<LevelSO, LevelSelectButtonUI> buttons = new();

        private void Update()
        {
            foreach (LevelListSO.Level level in mainMenuScene.LevelList.GetLevelStates())
            {
                if (!buttons.TryGetValue(level.LevelSO, out LevelSelectButtonUI button))
                {
                    // create missing button
                    button = Instantiate(buttonPrefab, levelButtonContainer);
                    button.OnClicked += LevelSelectButtonUI_OnClicked;
                    buttons.Add(level.LevelSO, button);
                }

                button.SetLevel(level);
            }
        }

        private void LevelSelectButtonUI_OnClicked(LevelSelectButtonUI button)
        {
            mainMenuScene.ToLevelDetails(button.LevelSO);
        }
    }
}
