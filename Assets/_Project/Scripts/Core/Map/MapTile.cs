using UnityEngine;
using UnityEngine.Tilemaps;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameJam
{
    [Serializable]
    public class MapTile : Tile
    {
        [Header("Config")]

        [Tooltip("If can be destroyed by lebros")]
        [SerializeField] public bool Destroyable = true;

        [Tooltip("If has flat top surface")]
        [SerializeField] public bool IsFlat = true;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MapTile))]
    public class MapTileEditor : Editor
    {
        private MapTile tile { get { return target as MapTile; } }

        [MenuItem("Assets/Create/Tiles/MapTile")]
        public static void CreateMapTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Map Tile", "New Map Tile", "Asset", "Save Map Tile");
            if (path == "")
                return;
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<MapTile>(), path);
        }
    }
#endif
}
