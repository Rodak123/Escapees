using UnityEngine;
using UnityEngine.Tilemaps;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameJam
{
    [Serializable]
    public class GridMapTile : MapTile
    {
        [Header("Grid")]
        [SerializeField, Min(1)] private int gridWidth = 1;
        [SerializeField, Min(1)] private int gridHeight = 1;
        [SerializeField] private Sprite[] sprites;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);

            if (sprites.Length == 0) return;

            int spriteX = LoopIndex(position.x, gridWidth);
            int spriteY = LoopIndex(-position.y, gridHeight);

            int index = spriteX + spriteY * gridWidth;
            index = Mathf.Clamp(index, 0, sprites.Length);

            Sprite sprite = sprites[index];

            if (sprite == null) return;

            tileData.sprite = sprite;
        }

        private int LoopIndex(int index, int count)
        {
            return ((index % count) + count) % count;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GridMapTile))]
    public class GridMapTileEditor : Editor
    {
        private GridMapTile tile { get { return target as GridMapTile; } }

        [MenuItem("Assets/Create/Tiles/GridMapTile")]
        public static void CreateGridMapTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Map Tile", "New Map Tile", "Asset", "Save Map Tile");
            if (path == "")
                return;
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GridMapTile>(), path);
        }
    }
#endif
}
