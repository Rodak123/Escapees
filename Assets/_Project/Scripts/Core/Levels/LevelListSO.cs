using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    [CreateAssetMenu(fileName = "LevelList", menuName = "Level List")]
    public class LevelListSO : ScriptableObject
    {
        public struct Level
        {
            public LevelSO LevelSO;
            public LevelData LevelData;
            public bool IsLocked;
        }

        public readonly IReadOnlyList<LevelState> QuickPlayOrder = new List<LevelState>()
        {
            LevelState.Unplayed,
            LevelState.Failed,
            LevelState.Complete,
        };

        [Header("Levels")]
        [SerializeField] private List<LevelSO> levels;

        public IReadOnlyList<LevelSO> Levels => levels;

        public IEnumerable<Level> GetLevelStates()
        {
            LevelData previousLevelData = null;
            for (int i = 0; i < levels.Count; i++)
            {
                LevelSO level = levels[i];

                LevelData data = LevelDataStorage.LoadLevelData(level);

                bool isLocked = !(previousLevelData?.IsCompleted ?? true);

                previousLevelData = data;

                yield return new()
                {
                    LevelSO = level,
                    LevelData = data,
                    IsLocked = isLocked,
                };
            }
        }

        public Level? GetQuickPlayLevel()
        {
            foreach (LevelState state in QuickPlayOrder)
            {
                foreach (Level levelState in GetLevelStates())
                {
                    if (levelState.IsLocked) continue;

                    if (levelState.LevelData.State == state)
                        return levelState;
                }
            }
            return null;
        }

        public bool TryGetNextLevel(LevelSO level, out LevelSO nextLevel)
        {
            for (int i = 0; i < levels.Count - 1; i++)
            {
                if (level == levels[i])
                {
                    nextLevel = levels[i + 1];
                    return true;
                }
            }

            nextLevel = default;
            return false;
        }
    }
}