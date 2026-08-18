using UnityEngine;

namespace GameJam
{
    public class MapSounds : MonoBehaviour
    {
        [SerializeField] private SoundEffect tileDestroyedSound;

        private Map map;

        private void Awake()
        {
            map = GameContext.Instance.Map;

            map.OnTileDestroyed += Map_OnTileDestroyed;
        }

        private void Map_OnTileDestroyed(Vector2Int cellPosition)
        {
            SFXManager.Instance.PlaySFX(tileDestroyedSound, 1);
        }
    }
}
