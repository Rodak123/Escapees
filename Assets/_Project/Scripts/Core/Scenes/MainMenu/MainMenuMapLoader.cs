using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameJam
{
    public class MainMenuMapLoader : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;

        private void Start()
        {
            Map map = GameContext.Instance.Map;

            for (int x = 0; x < map.Size.x; x++)
            {
                for (int y = 0; y < map.Size.y; y++)
                {
                    MapTile tile = tilemap.GetTile<MapTile>(new(x, y));
                    map.TrySetTileAt(new(x, y), tile);
                }
            }
        }
    }
}
