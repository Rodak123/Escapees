using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    [RequireComponent(typeof(Camera))]
    public class MainMenuCamera : MonoBehaviour
    {
        [Serializable]
        public struct SnapPoint
        {
            public MainMenuScene.MenuState State;
            public Vector2Int Position;
        }

        [SerializeField] private MainMenuScene mainMenuScene;
        [SerializeField] private float moveSpeed = 16;
        [SerializeField] private SnapPoint[] snapPoints;

        private readonly Dictionary<MainMenuScene.MenuState, Vector2Int> statePoints = new();

        private void Awake()
        {
            foreach (SnapPoint snapPoint in snapPoints)
            {
                statePoints.Add(snapPoint.State, snapPoint.Position);
            }
        }

        private void Start()
        {
            transform.position = GetTargetPosition();
        }

        private void Update()
        {
            Vector3 targetPosition = GetTargetPosition();

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            Vector2 camDelta = InputManager.Instance.ReadCameraMoveDelta();
            if (Vector3.Distance(transform.position, targetPosition) <= 1f)
            {
                switch (mainMenuScene.State)
                {
                    case MainMenuScene.MenuState.MainMenu:
                        if (camDelta.y > 0.5) mainMenuScene.ToSettings();
                        if (camDelta.x > 0.5) mainMenuScene.ToLevelSelect();
                        break;
                    case MainMenuScene.MenuState.LevelSelect:
                        if (camDelta.x < -0.5) mainMenuScene.ToMainMenu();
                        if (camDelta.x > 0.5 && mainMenuScene.QuickPlayLevel.HasValue) mainMenuScene.ToLevelDetails(mainMenuScene.QuickPlayLevel.Value.LevelSO);
                        break;
                    case MainMenuScene.MenuState.Settings:
                        if (camDelta.y < -0.5) mainMenuScene.ToMainMenu();
                        break;
                    case MainMenuScene.MenuState.LevelDetails:
                        if (camDelta.x < -0.5) mainMenuScene.ToLevelSelect();
                        break;
                }
            }
        }

        private Vector3 GetTargetPosition()
        {
            if (statePoints.TryGetValue(mainMenuScene.CurrentMenuState, out Vector2Int position))
                return new(position.x, position.y, transform.position.z);
            return transform.position;
        }
    }
}
