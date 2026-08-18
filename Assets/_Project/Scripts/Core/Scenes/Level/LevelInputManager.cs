using UnityEngine;

namespace GameJam
{
    public class LevelInputManager : MonoBehaviour
    {
        [SerializeField] private LevelScene levelScene;

        private void Awake()
        {
            levelScene.OnGameStateChanged += LevelScene_OnGameStateChanged;
        }

        private void Update()
        {
            if (InputManager.Instance.WasGameQuickRestartPressedThisFrame())
            {
                levelScene.RestartLevel();
            }

            if (InputManager.Instance.WasGameBackPressedThisFrame() || InputManager.Instance.WasGameTogglePausePressedThisFrame())
            {
                if (levelScene.State == LevelScene.GameState.Paused) levelScene.Unpause();
                else levelScene.Pause();
            }

            if (levelScene.State == LevelScene.GameState.Ended && InputManager.Instance.WasGameNextLevelPressedThisFrame())
            {
                levelScene.GoToNextLevel();
            }
        }

        private void LevelScene_OnGameStateChanged(LevelScene.GameState state)
        {
            if (state == LevelScene.GameState.Playing)
            {
                InputManager.Instance.PlayerActions.Enable();
                InputManager.Instance.CameraActions.Enable();
            }
            else
            {
                InputManager.Instance.CameraActions.Disable();
                InputManager.Instance.PlayerActions.Disable();
            }
        }

    }
}
