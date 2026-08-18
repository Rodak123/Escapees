using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    [CreateAssetMenu(fileName = "Level", menuName = "Level")]
    public class LevelSO : ScriptableObject
    {
        [Header("Info")]
        [SerializeField] private LevelAreaSO area;
        [SerializeField] private string levelName = "Level";
        [SerializeField, Min(0)] private int levelNumber = 0;

        [Header("Lebro")]
        [SerializeField, Min(0)] private int totalLebroCount = 1;
        [SerializeField, Min(0)] private int requiredLebroSavedCount = 1;

        [Header("Level")]
        [SerializeField] private LevelLayout levelLayout;
        [SerializeField] private List<ToolSO> availableTools;

        [Header("Music")]
        [SerializeField] private MusicSO music;

        public string LevelName => levelName;
        public int LevelNumber => levelNumber;
        public LevelAreaSO Area => area;

        public int TotalLebroCount => totalLebroCount;
        public int RequiredLebroSavedCount => requiredLebroSavedCount;

        public LevelLayout LevelLayout => levelLayout;
        public IReadOnlyList<ToolSO> AvailableTools => availableTools;

        public MusicSO Music => music;

        public void Validate()
        {
            if (LevelLayout == null) throw new ArgumentNullException($"{nameof(levelLayout)} of level '{this}' was not set");
            if (requiredLebroSavedCount > totalLebroCount) Debug.LogWarning($"{nameof(requiredLebroSavedCount)} is bigger than {nameof(totalLebroCount)} ({requiredLebroSavedCount} > {totalLebroCount})");
        }

        public string GetLevelString(bool includeName)
        {
            return $"{area.AreaNumber}-{levelNumber}" + (includeName ? $" {LevelName}" : string.Empty);
        }

        public override string ToString()
        {
            return GetLevelString(true);
        }
    }
}