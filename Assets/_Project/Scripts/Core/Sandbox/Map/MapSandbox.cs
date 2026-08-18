using UnityEngine;

namespace GameJam
{
    public class MapSandbox : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Chance that a pixel is destroyed completely when tile is destroyed")]
        [SerializeField, Range(0, 1)] private float pixelDestroyChance = 0;

        private Map map;
        private Simulation simulation;

        public Simulation Simulation => simulation;

        private void Awake()
        {
            map = GameContext.Instance.Map;
            simulation = new(map.Size.x * Map.CellSize, map.Size.y * Map.CellSize);
        }

        private void OnEnable()
        {
            map.OnTilePlaced += Map_OnTilePlaced_OnTileBuilded;
            map.OnTileBuilded += Map_OnTilePlaced_OnTileBuilded;
            map.OnTileDestroyed += Map_OnTileDestroyed;
        }

        private void OnDisable()
        {
            map.OnTilePlaced -= Map_OnTilePlaced_OnTileBuilded;
            map.OnTileBuilded -= Map_OnTilePlaced_OnTileBuilded;
            map.OnTileDestroyed -= Map_OnTileDestroyed;
        }

        private void Update()
        {
            simulation.Tick();
        }

        private void Map_OnTilePlaced_OnTileBuilded(Vector2Int position)
        {
            Vector2Int worldPosition = position * Map.CellSize;

            MapTile tile = map.GetTileAt(position);

            if (tile == null || tile.sprite == null)
                return;

            Sprite sprite = tile.sprite;
            Texture2D texture = sprite.texture;

            Rect rect = sprite.rect;
            int rectX = Mathf.FloorToInt(rect.x);
            int rectY = Mathf.FloorToInt(rect.y);
            int width = Mathf.FloorToInt(rect.width);
            int height = Mathf.FloorToInt(rect.height);

            Color[] pixels = texture.GetPixels(rectX, rectY, width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixelColor = pixels[x + y * width];

                    if (pixelColor.a == 0)
                        continue;

                    Vector2Int pixelPosition = worldPosition + new Vector2Int(x, y);

                    simulation.TrySetElementAt(pixelPosition.x, pixelPosition.y, new ColoredStone(pixelColor));
                }
            }
        }

        private void Map_OnTileDestroyed(Vector2Int position)
        {
            Vector2Int worldPosition = position * Map.CellSize;

            for (int x = 0; x < Map.CellSize; x++)
            {
                for (int y = 0; y < Map.CellSize; y++)
                {
                    Vector2Int pixelPosition = worldPosition + new Vector2Int(x, y);

                    if (!simulation.TryGetElementAt(pixelPosition.x, pixelPosition.y, out ColoredStone coloredStone))
                        continue;

                    bool isDestroyed = Random.value < pixelDestroyChance;

                    if (isDestroyed) simulation.TryRemoveElementAt(pixelPosition.x, pixelPosition.y);
                    else coloredStone.Pulverize();
                }
            }
        }
    }
}
