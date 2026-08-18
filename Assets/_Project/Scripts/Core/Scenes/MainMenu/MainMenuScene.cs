using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam
{
    public class MainMenuScene : MonoBehaviour
    {
        [Serializable]
        public enum MenuState
        {
            MainMenu,
            LevelSelect,
            Settings,
            LevelDetails
        }

        public static LevelListSO CurrentLevelList;
        public static LevelSO TargetLevel;

        [SerializeField] private LevelListSO levelList;

        [Header("Debug")]
        [SerializeField] private MenuState menuState = MenuState.MainMenu;

        private LevelSO selectedLevel;
        private LevelListSO.Level? quickPlayLevel;

        public MenuState CurrentMenuState => menuState;
        public LevelSO SelectedLevel => selectedLevel;
        public LevelListSO.Level? QuickPlayLevel => quickPlayLevel;

        public LevelListSO LevelList => levelList;

        public MenuState State => menuState;

        public event Action<MainMenuScene> OnMenuStateChanged;

        private void Awake()
        {
            if (CurrentLevelList == null)
            {
                CurrentLevelList = levelList;
            }
            else
            {
                levelList = CurrentLevelList;
            }

            if (TargetLevel != null)
            {
                ToLevelDetails(TargetLevel);
                TargetLevel = null;
            }

            quickPlayLevel = levelList.GetQuickPlayLevel();
        }

        private void Update()
        {
            quickPlayLevel = levelList.GetQuickPlayLevel();

            if (InputManager.Instance.WasGameBackPressedThisFrame())
            {
                switch (menuState)
                {
                    case MenuState.MainMenu:
                        break;
                    case MenuState.LevelSelect:
                        ToMainMenu();
                        break;
                    case MenuState.Settings:
                        ToMainMenu();
                        break;
                    case MenuState.LevelDetails:
                        ToLevelSelect();
                        break;
                }
            }
        }

        private void ChangeMenuState(MenuState newMenuState)
        {
            menuState = newMenuState;
            OnMenuStateChanged?.Invoke(this);
        }

        public void ToMainMenu()
        {
            if (menuState == MenuState.MainMenu) return;
            ChangeMenuState(MenuState.MainMenu);
        }

        public void ToLevelSelect()
        {
            if (menuState == MenuState.LevelSelect) return;
            ChangeMenuState(MenuState.LevelSelect);
        }

        public void ToLevelDetails(LevelSO level)
        {
            selectedLevel = level;
            ChangeMenuState(MenuState.LevelDetails);
        }

        public void QuickPlay()
        {
            if (!quickPlayLevel.HasValue) return;
            LevelScene.TargetLevel = quickPlayLevel.Value.LevelSO;
            SceneManager.LoadScene((int)GameScene.LevelScene);
        }

        public void PlaySelectedLevel()
        {
            LevelScene.TargetLevel = selectedLevel;
            SceneManager.LoadScene((int)GameScene.LevelScene);
        }

        public void ToSettings()
        {
            if (menuState == MenuState.Settings) return;
            ChangeMenuState(MenuState.Settings);
        }

        public void DeleteProgress()
        {
            foreach (LevelSO level in levelList.Levels)
            {
                LevelDataStorage.DeleteLevelData(level);
            }
            SceneManager.LoadScene((int)GameScene.MainMenuScene);
        }

        public void Exit()
        {
            ApplicationManager.Instance.Quit();
        }
    }
}
