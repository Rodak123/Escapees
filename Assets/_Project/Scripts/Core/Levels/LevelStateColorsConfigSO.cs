using System;
using UnityEngine;

namespace GameJam
{
    [CreateAssetMenu(fileName = "LevelStateColorsConfig", menuName = "Level State Colors Config")]
    public class LevelStateColorsConfigSO : ScriptableObject
    {
        [Serializable]
        private struct LevelStateColor
        {
            public LevelState State;
            public Color Color;
        }

        [SerializeField] private Color lockedColor = Color.gray;
        [SerializeField] private LevelStateColor[] levelStateColors;

        public Color GetStateColor(LevelState state, bool isLocked)
        {
            if (isLocked) return lockedColor;
            foreach (LevelStateColor color in levelStateColors)
            {
                if (color.State == state) return color.Color;
            }
            return Color.pink;
        }
    }
}