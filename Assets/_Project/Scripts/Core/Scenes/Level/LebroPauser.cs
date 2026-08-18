using UnityEngine;

namespace GameJam
{
    public class LebroPauser : MonoBehaviour
    {
        [SerializeField] private LevelScene levelScene;

        private void Awake()
        {
            levelScene.OnGameStateChanged += LevelScene_OnGameStateChanged;
        }

        private void LevelScene_OnGameStateChanged(LevelScene.GameState state)
        {
            if (state == LevelScene.GameState.Paused)
            {
                Time.timeScale = 0;
                GameContext.Instance.LebroManager.PauseLebros();
            }
            else
            {
                Time.timeScale = 1;
                GameContext.Instance.LebroManager.UnpauseLebros();
            }
        }

        private void OnDestroy()
        {
            Time.timeScale = 1;
        }

    }
}
