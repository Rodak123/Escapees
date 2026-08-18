using UnityEngine;
using UnityEngine.Tilemaps;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameJam
{
    [Serializable]
    public class TiledMapTile : MapTile
    {
        [Serializable]
        public struct SpriteRow
        {
            public Sprite[] RowSprites;
        }

        [Header("Tiling")]
        [SerializeField] private SpriteRow[] spriteRows;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);

            if (spriteRows.Length == 0) return;
            int spriteYIndex = LoopIndex(position.y, spriteRows.Length);

            SpriteRow spriteRow = spriteRows[spriteYIndex];

            if (spriteRow.RowSprites.Length == 0) return;

            int spriteXIndex = LoopIndex(position.x, spriteRow.RowSprites.Length);
            Sprite sprite = spriteRow.RowSprites[spriteXIndex];

            if (sprite == null) return;

            tileData.sprite = sprite;
        }

        private int LoopIndex(int index, int count)
        {
            return ((index % count) + count) % count;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TiledMapTile))]
    public class TiledMapTileEditor : Editor
    {
        private TiledMapTile tile { get { return target as TiledMapTile; } }

        [MenuItem("Assets/Create/Tiles/TiledMapTile")]
        public static void CreateTiledMapTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Map Tile", "New Map Tile", "Asset", "Save Map Tile");
            if (path == "")
                return;
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TiledMapTile>(), path);
        }
    }
#endif
}
