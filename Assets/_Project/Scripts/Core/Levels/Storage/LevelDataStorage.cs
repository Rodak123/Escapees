using UnityEngine;

namespace GameJam
{
    public static class LevelDataStorage
    {
        public const string PREFS_STORE_KEY_PREFIX = "LEVEL_DATA_STORAGE";

        private static string GetStoreKey(string id) => $"{PREFS_STORE_KEY_PREFIX}_{id}";

        public static string GetLevelId(LevelSO level)
        {
            return $"{level.Area.AreaName}:{level.Area.AreaNumber}-{level.LevelName}:{level.LevelNumber}";
        }

        public static void SaveLevelData(LevelData levelData)
        {
            string json = JsonUtility.ToJson(levelData);

            PlayerPrefs.SetString(GetStoreKey(levelData.LevelId), json);
            PlayerPrefs.Save();
        }

        public static LevelData LoadLevelData(LevelSO level)
        {
            string id = GetLevelId(level);
            string json = PlayerPrefs.GetString(GetStoreKey(id), "");

            if (json.Length == 0)
            {
                return new()
                {
                    LevelId = id,
                };
            }

            return JsonUtility.FromJson<LevelData>(json);
        }

        public static void DeleteLevelData(LevelSO level)
        {
            string id = GetLevelId(level);
            PlayerPrefs.DeleteKey(GetStoreKey(id));
            PlayerPrefs.Save();
        }

    }
}
