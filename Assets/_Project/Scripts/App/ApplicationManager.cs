using System;
using System.Collections.Generic;
using Rodak.Utils.Singleton;
using UnityEngine;

namespace GameJam
{
    public class ApplicationManager : SingletonMonoBehaviour<ApplicationManager>
    {
        [Serializable]
        public enum DisplayMode
        {
            Fullscreen,
            Borderless,
            Windowed,
            MaximizedWindow
        }

        [Serializable]
        public struct Resolution
        {
            public int Width;
            public int Height;

            public Resolution(int w, int h)
            {
                Width = w;
                Height = h;
            }
        }

        private const string ResolutionKey = "DisplayResolution";
        private const string DisplayModeKey = "DisplayMode";
        private const string RunInBGKey = "RunInBG";

        [Header("Display")]
        [SerializeField]
        private List<Resolution> resolutions = new()
        {
            new(1024, 768),
            new(1280, 720),
            new(1366, 768),
            new(1280, 800),
            new(1536, 864),
            new(1600, 900),
            new(1920, 1080),
            new(1920, 1200),
            new(2560, 1440),
            new(2560, 1600),
            new(3440, 1440),
            new(3840, 2160)
        };
        [SerializeField]
        private DisplayMode[] displayModes = new DisplayMode[]
        {
            DisplayMode.Borderless,
            DisplayMode.Fullscreen,
            DisplayMode.Windowed,
            DisplayMode.MaximizedWindow
        };

        [Header("Performance")]
        [SerializeField] private bool runInBG = true;

        private int currentResolutionIndex = 0;
        private int currentDisplayModeIndex = 0;

        public Resolution[] Resolutions => resolutions.ToArray();
        public DisplayMode[] DisplayModes => displayModes;

        public int CurrentResolutionIndex => currentResolutionIndex;
        public int CurrentDisplayModeIndex => currentDisplayModeIndex;

        public event Action OnSettingsChanged;

        protected override void Awake()
        {
            base.Awake();

            LoadSettings();
            ApplyAllSettings();
        }

        private void Update()
        {
            if (InputManager.Instance.WasApplicationQuitPressedThisFrame())
            {
                Quit();
            }
        }

        private void UpdateWindow()
        {
#if !UNITY_WEBGL
            Resolution resolution = GetResolution();
            Screen.SetResolution(resolution.Width, resolution.Height, GetFullScreenMode());
#endif
        }

        private void UpdateBackgroundBehavior()
        {
            Application.runInBackground = runInBG;
        }

        private FullScreenMode GetFullScreenMode()
        {
            return GetDisplayMode() switch
            {
                DisplayMode.Fullscreen => FullScreenMode.ExclusiveFullScreen,
                DisplayMode.Borderless => FullScreenMode.FullScreenWindow,
                DisplayMode.Windowed => FullScreenMode.Windowed,
                DisplayMode.MaximizedWindow => FullScreenMode.MaximizedWindow,
                _ => FullScreenMode.FullScreenWindow,
            };
        }

        public Resolution GetResolution() => resolutions[currentResolutionIndex];

        public void SetResolution(int index)
        {
            if (index < 0 || index >= resolutions.Count) return;
            currentResolutionIndex = index;
            SaveSettings();
        }

        public void ResetResolution() => SetResolution(0);

        public DisplayMode GetDisplayMode() => displayModes[currentDisplayModeIndex];

        public void SetDisplayMode(int index)
        {
            if (index < 0 || index >= displayModes.Length) return;
            currentDisplayModeIndex = index;
            SaveSettings();
        }

        public void ResetDisplayMode() => SetDisplayMode(0);

        public void SetRunInBackground(bool run)
        {
            runInBG = run;
            SaveSettings();
        }

        private int GetClosestResolutionIndex()
        {
            int closest = 0;
            int minDifference = int.MaxValue;

            for (int i = 0; i < resolutions.Count; i++)
            {
                Resolution resolution = resolutions[i];
                int diffX = Screen.currentResolution.width - resolution.Width;
                int diffY = Screen.currentResolution.height - resolution.Height;

                if (diffX < 0 || diffY < 0) continue;

                if (diffX == 0 && diffY == 0) return i;

                int currentTotalDiff = diffX + diffY;
                if (currentTotalDiff < minDifference)
                {
                    minDifference = currentTotalDiff;
                    closest = i;
                }
            }

            return closest;
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetInt(ResolutionKey, currentResolutionIndex);
            PlayerPrefs.SetInt(DisplayModeKey, currentDisplayModeIndex);
            PlayerPrefs.SetInt(RunInBGKey, runInBG ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void LoadSettings()
        {
            currentResolutionIndex = PlayerPrefs.GetInt(ResolutionKey, GetClosestResolutionIndex());
            currentDisplayModeIndex = PlayerPrefs.GetInt(DisplayModeKey, 0);
            runInBG = PlayerPrefs.GetInt(RunInBGKey, 1) == 1;
        }

        public void ApplyAllSettings()
        {
            SaveSettings();
            UpdateWindow();
            UpdateBackgroundBehavior();
            OnSettingsChanged?.Invoke();
        }

        public void Quit()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}