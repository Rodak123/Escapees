using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    [Serializable]
    public class LevelData
    {
        public string LevelId;
        public LevelState State = LevelState.Unplayed;
        public List<LevelAttempt> Attempts = new();

        public bool IsCompleted => (int)State >= (int)LevelState.Complete;
        public bool TryGetBestTimeSeconds(out float bestTime, LevelState minimumState = LevelState.Failed)
        {
            if (Attempts.Count == 0)
            {
                bestTime = default;
                return false;
            }

            bestTime = float.MaxValue;
            foreach (LevelAttempt attempt in Attempts)
            {
                if ((int)attempt.State < (int)minimumState) continue;
                bestTime = Mathf.Min(bestTime, attempt.TimeSeconds);
            }

            if (bestTime == float.MaxValue)
            {
                bestTime = default;
                return false;
            }

            return true;
        }
    }
}
