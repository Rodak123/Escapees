using UnityEngine;

namespace Rodak.Utils
{
    /// <summary>
    /// A camera letterboxing utility script.
    /// Source: https://gist.github.com/cmiddlebrook/395c0f638393addea81ec454aea62811
    /// </summary>
    [ExecuteAlways, RequireComponent(typeof(Camera))]
    public class CameraLetterbox : MonoBehaviour
    {
        [SerializeField] private int _targetXAspect = 16;
        [SerializeField] private int _targetYAspect = 9;
        private float _targetAspectRatio;

        private Camera _camera;
        private float _lastWidth;
        private float _lastHeight;
        private Rect _originalCameraRect;

        void Awake()
        {
            _targetAspectRatio = (float)_targetXAspect / _targetYAspect;
            _camera = GetComponent<Camera>();
            _originalCameraRect = _camera.rect;
            UpdateCamera();
        }

        void Update()
        {
            if (Screen.width != _lastWidth || Screen.height != _lastHeight)
            {
                UpdateCamera();
            }
        }

        void OnDisable()
        {
            if (_camera != null)
                _camera.rect = _originalCameraRect;
        }

        void UpdateCamera()
        {
            if (Screen.width <= 0 || Screen.height <= 0) return;

            _lastWidth = Screen.width;
            _lastHeight = Screen.height;

            float windowAspectRatio = (float)Screen.width / Screen.height;
            float scaleHeight = windowAspectRatio / _targetAspectRatio;

            Rect rect = new();

            if (scaleHeight < 1.0f)
            {
                rect.width = 1.0f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1.0f - scaleHeight) / 2.0f;
            }
            else
            {
                float scaleWidth = 1.0f / scaleHeight;
                rect.width = scaleWidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scaleWidth) / 2.0f;
                rect.y = 0;
            }

            _camera.rect = rect;
        }
    }
}