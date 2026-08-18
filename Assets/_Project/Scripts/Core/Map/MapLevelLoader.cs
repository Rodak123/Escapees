using UnityEngine;

namespace GameJam
{
    public class MapLevelLoader : MonoBehaviour
    {
        private Map map;
        private ToolBelt toolBelt;

        private void Awake()
        {
            map = GameContext.Instance.Map;
            toolBelt = GameContext.Instance.ToolBelt;
        }

        public GameObject LoadLevel(LevelSO level)
        {
            map.Clear();

            if (level.LevelLayout.LebroStart != null) level.LevelLayout.LebroStart.SpawnUntil = level.TotalLebroCount;

            GameObject world = Instantiate(level.LevelLayout.World);
            world.transform.position = map.Offset; // align with the map

            for (int x = 0; x < map.Size.x; x++)
            {
                for (int y = 0; y < map.Size.y; y++)
                {
                    MapTile tile = level.LevelLayout.LevelTilemap.GetTile<MapTile>(new(x, y));
                    map.TrySetTileAt(new(x, y), tile);
                }
            }

            if (level.AvailableTools.Count > 0) toolBelt.LoadTools(level.AvailableTools);

            map.CompressBounds();

            return world;
        }
    }
}
