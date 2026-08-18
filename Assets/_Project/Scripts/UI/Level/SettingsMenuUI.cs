using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameJam
{
    public class SettingsMenuUI : MonoBehaviour
    {
        [SerializeField] private LevelScene levelScene;

        [Header("UI")]
        [SerializeField] private GameObject settingsMenuContainer;

        [SerializeField] private Button exitButton;

        private void Awake()
        {
            exitButton.onClick.AddListener(() => SetSettingsVisibility(false));
        }

        public void SetSettingsVisibility(bool enabled)
        {
            settingsMenuContainer.SetActive(enabled);
        }
    }
}
