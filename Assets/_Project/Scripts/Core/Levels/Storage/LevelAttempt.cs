using System;

namespace GameJam
{
    [Serializable]
    public class LevelAttempt
    {
        public static LevelState EvaluateAttemptState(LevelAttempt attempt, LevelSO level)
        {
            if (attempt.LebrosSaved == level.TotalLebroCount)
            {
                return LevelState.Perfect;
            }

            if (attempt.LebrosSaved >= level.RequiredLebroSavedCount)
            {
                return LevelState.Complete;
            }

            return LevelState.Failed;
        }

        public int LebrosSaved;
        public float TimeSeconds;
        public LevelState State = LevelState.Unplayed;

        public bool IsCompleted => (int)State >= (int)LevelState.Complete;
    }
}
