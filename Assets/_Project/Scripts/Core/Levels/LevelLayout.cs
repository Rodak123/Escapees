using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameJam
{
    public class LevelLayout : MonoBehaviour
    {
        [SerializeField] private Tilemap levelTilemap;
        [SerializeField] private GameObject world;

        [Header("Lebro")]
        [SerializeField] private LebroStart lebroStart;
        [SerializeField] private LebroEnd lebroEnd;

        public Tilemap LevelTilemap => levelTilemap;
        public GameObject World => world;

        public LebroStart LebroStart => lebroStart;
        public LebroEnd LebroEnd => lebroEnd;

    }
}
