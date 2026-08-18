using System;
using UnityEngine;

namespace GameJam
{
    [RequireComponent(typeof(Camera))]
    public class MapCameraMovement : MonoBehaviour
    {
        [SerializeField] private LevelScene levelScene;

        [Header("Config")]
        [SerializeField] private float moveDeltaSensitivity = 8;

        private Camera cam;
        private Map map;

        private Vector2 position;

        private void Awake()
        {
            cam = GetComponent<Camera>();

            map = GameContext.Instance.Map;
            position = transform.position;

            levelScene.OnLevelLoaded += LevelScene_OnLevelLoaded;
        }

        private void LevelScene_OnLevelLoaded(LevelScene scene)
        {
            if (levelScene.CurrentLevelStart == null) return;
            position = levelScene.CurrentLevelStart.transform.position;
            UpdatePosition();
        }

        private void Update()
        {
            MoveCamera();
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            LimitPosition();
            Vector2 pixelPosition = position.RoundToInt();
            transform.position = new(pixelPosition.x, pixelPosition.y, transform.position.z);
        }

        private void MoveCamera()
        {
            Vector2 moveDelta = InputManager.Instance.ReadCameraMoveDelta();
            position += moveDeltaSensitivity * Time.deltaTime * moveDelta;
        }

        private void LimitPosition()
        {
            Bounds mapBounds = map.MapBounds;

            float height = cam.orthographicSize * 2f;
            float width = height * cam.aspect;

            Vector3 halfCameraSize = new Vector2(width, height) / 2f;

            Vector2 cameraMin = mapBounds.min + map.Offset + halfCameraSize;
            Vector2 cameraMax = mapBounds.max + map.Offset - halfCameraSize;

            position.x = Mathf.Clamp(position.x, cameraMin.x, cameraMax.x);
            position.y = Mathf.Clamp(position.y, cameraMin.y, cameraMax.y);
        }
    }
}
