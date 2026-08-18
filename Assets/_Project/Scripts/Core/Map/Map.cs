using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameJam
{
    public class Map : MonoBehaviour
    {
        public static int CellSize = 5;

        [SerializeField] private Grid grid;
        [SerializeField] private Vector2Int size = new(64, 64);

        [Header("Tilemaps")]
        [SerializeField] private Tilemap levelTilemap;
        [SerializeField] private Tilemap buildableTilemap;

        [Header("Debug")]
        [ReadOnly, SerializeField] private Vector3 debugBoundsSize;
        [ReadOnly, SerializeField] private Vector3 debugBoundsMin;
        [ReadOnly, SerializeField] private Vector3 debugBoundsMax;

        public Vector2Int Size => size;
        public Vector3 Offset => grid.transform.position;
        public Bounds MapBounds => levelTilemap.localBounds;

        public event Action<Vector2Int> OnTilePlaced;
        public event Action<Vector2Int> OnTileBuilded;
        public event Action<Vector2Int> OnTileDestroyed;

        private void Awake()
        {
            grid.cellSize = new(CellSize, CellSize, CellSize);

            size.x = Mathf.Max(1, size.x);
            size.y = Mathf.Max(1, size.y);
        }

        private void Update()
        {
            debugBoundsSize = MapBounds.size;
            debugBoundsMin = MapBounds.min;
            debugBoundsMax = MapBounds.max;
        }

        public bool IsValidPosition(Vector2Int position)
        {
            if (position.x < 0 || position.x >= Size.x) return false;
            if (position.y < 0 || position.y >= Size.y) return false;
            return true;
        }

        public MapTile GetTileAt(Vector2Int position)
        {
            if (!IsValidPosition(position)) throw new ArgumentOutOfRangeException($"{position} is outside the map.");

            Vector3Int pos = new(position.x, position.y);

            MapTile tile = buildableTilemap.GetTile<MapTile>(pos);
            if (tile != null)
                return tile;

            return levelTilemap.GetTile<MapTile>(pos);
        }

        public bool TryGetTileAt(Vector2Int position, out MapTile tile)
        {
            if (!IsValidPosition(position))
            {
                tile = default;
                return false;
            }

            Vector3Int pos = new(position.x, position.y);

            tile = buildableTilemap.GetTile<MapTile>(pos);
            if (tile == null) tile = levelTilemap.GetTile<MapTile>(pos);

            return true;
        }

        public Vector2Int WorldToCell(Vector2 worldPosition)
        {
            Vector3Int cellPosition = grid.WorldToCell(worldPosition);
            return new(cellPosition.x, cellPosition.y);
        }

        public Vector2 CellToWorld(Vector2Int cellPosition)
        {
            Vector3 worldPosition = grid.CellToWorld(new(cellPosition.x, cellPosition.y));
            return new(worldPosition.x, worldPosition.y);
        }

        public bool TrySetTileAt(Vector2Int position, MapTile tile)
        {
            if (tile == null || !IsValidPosition(position))
                return false;

            levelTilemap.SetTile(new(position.x, position.y), tile);
            OnTilePlaced?.Invoke(position);
            return true;
        }

        public bool TryDestroyTileAt(Vector2Int position)
        {
            if (!IsValidPosition(position))
                return false;

            levelTilemap.SetTile(new(position.x, position.y), null);
            buildableTilemap.SetTile(new(position.x, position.y), null);
            OnTileDestroyed?.Invoke(position);
            return true;
        }

        public bool TryBuildTileAt(Vector2Int position, MapTile tile)
        {
            if (tile == null || !IsValidPosition(position))
                return false;

            buildableTilemap.SetTile(new(position.x, position.y), tile);
            OnTileBuilded?.Invoke(position);
            return true;
        }

        public void Clear()
        {
            for (int y = 0; y < Size.y; y++)
            {
                for (int x = 0; x < Size.x; x++)
                {
                    levelTilemap.SetTile(new(x, y), null);
                }
            }
        }

        public void CompressBounds()
        {
            levelTilemap.CompressBounds();
            buildableTilemap.CompressBounds();
        }
    }
}
