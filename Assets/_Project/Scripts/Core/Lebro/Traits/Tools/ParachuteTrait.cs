using UnityEngine;

namespace GameJam
{
    public class ParachuteTrait : AToolTrait
    {
        [Header("Visual")]
        [SerializeField] private Sprite parachuteSprite;

        [Space]
        [SerializeField] private GameObject lebroParachuteSpritePrefab;

        [Header("Sounds")]
        [SerializeField] private SoundEffect pickUpSound;
        [SerializeField] private SoundEffect dropSound;

        [Header("Config")]
        [SerializeField] private float fallSpeedScale = 0.3f;

        private float previousFallSpeedScale;

        private void ChangeCustomVisual(bool enable)
        {
            if (enable)
            {
                Lebro.Visual.EnableCustomVisualFor(LebroVisual.SpriteType.Falling, lebroParachuteSpritePrefab);
            }
            else
            {
                Lebro.Visual.DisableCustomVisualFor(LebroVisual.SpriteType.Falling);
            }
        }

        protected override void UpdateToolTrait()
        {
            if (Lebro.Controller.IsGrounded)
            {
                // remove parachute when hitting surface
                CancelTrait(0);
                Lebro.Sounds.PlaySFX(dropSound);
            }
        }

        protected override int GetPlayerCancelToken()
        {
            return -1;
        }

        protected override void CancelTrait(int token)
        {
            base.CancelTrait(token);

            Lebro.Controller.EnableLethalFalls = true;

            Lebro.Controller.FallVelocityScale = previousFallSpeedScale;

            ChangeCustomVisual(false);
        }

        public override void Apply(Vector2 obtainPosition)
        {
            Lebro.Controller.EnableLethalFalls = false;

            previousFallSpeedScale = Lebro.Controller.FallVelocityScale;
            Lebro.Controller.FallVelocityScale = fallSpeedScale;

            ChangeCustomVisual(true);

            Lebro.Sounds.PlaySFX(pickUpSound);
        }

        protected override SpriteRendererConfig GetIconConfig()
        {
            return new()
            {
                Sprite = parachuteSprite,
            };
        }

        public override void LoadSettingsState(int index)
        {
            // nothing
        }

        public override int GetNextSettingsStateIndex(int currentIndex)
        {
            return 0;
        }

        public override bool CanBeGivenTo(Lebro lebro)
        {
            return !lebro.Controller.IsGrounded;
        }

        public override bool CanBePlacedAt(Map map, Vector2Int cellPosition)
        {
            Vector2Int below = cellPosition + Vector2Int.down;

            if (!map.TryGetTileAt(below, out MapTile tileBelow))
                return false; // out of bounds

            if (tileBelow != null)
                return false; // tile is under

            return true;
        }
    }
}
