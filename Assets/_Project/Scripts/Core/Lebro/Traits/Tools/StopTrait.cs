using UnityEngine;

namespace GameJam
{
    public class StopTrait : AToolTrait
    {
        [SerializeField] private ToolSO stopTool;

        [Header("Visual")]
        [SerializeField] private Sprite stopSprite;

        [Space]
        [SerializeField] private GameObject lebroStopSpritePrefab;

        [Header("Sounds")]
        [SerializeField] private SoundEffect notHereSound;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!collider.gameObject.TryGetComponent(out LebroController lebroController))
                return;

            lebroController.TurnAround();
            Lebro.Sounds.PlaySFX(notHereSound);
        }

        private void ChangeCustomVisual(bool enable)
        {
            if (enable)
            {
                Lebro.Visual.EnableCustomVisualFor(LebroVisual.SpriteType.Standing, lebroStopSpritePrefab);
            }
            else
            {
                Lebro.Visual.DisableCustomVisualFor(LebroVisual.SpriteType.Standing);
            }
        }

        protected override void UpdateToolTrait()
        {
            // nothing
        }

        protected override int GetPlayerCancelToken()
        {
            if (InputManager.Instance.WasPlayerInteractSecondaryPressedThisFrame())
            {
                // 0 - left, 1 - right
                Vector3 mouse = InputManager.Instance.ReadMouseWorldPosition();
                if (mouse.x < Lebro.Controller.PixelPosition.x + Lebro.Controller.Size.x / 2)
                    return 0;
                else
                    return 1;
            }
            return -1;
        }

        protected override void CancelTrait(int token)
        {
            base.CancelTrait(token);

            Lebro.Controller.CurrentMovementGoal = token == 0
                ? LebroController.MovementGoal.WalkLeft
                : LebroController.MovementGoal.WalkRight;

            ChangeCustomVisual(false);

            if (GameContext.Instance.ToolBelt != null)
            {
                GameContext.Instance.ToolBelt.AddTool(stopTool);
            }
        }

        public override void Apply(Vector2 obtainPosition)
        {
            ChangeCustomVisual(true);

            Lebro.Controller.CurrentMovementGoal = LebroController.MovementGoal.Standing;

            Map map = GameContext.Instance.Map;

            Vector2Int cellPosition = map.WorldToCell(obtainPosition);
            Vector2 worldPosition = map.CellToWorld(cellPosition);
            Lebro.Controller.transform.position = worldPosition + Vector2.right;
        }

        protected override SpriteRendererConfig GetIconConfig()
        {
            return new()
            {
                Sprite = stopSprite,
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
            return lebro.Controller.IsGrounded;
        }

        public override bool CanBePlacedAt(Map map, Vector2Int cellPosition)
        {
            Vector2Int below = cellPosition + Vector2Int.down;

            if (!map.TryGetTileAt(below, out MapTile tileBelow) || tileBelow == null)
                return false; // no tile is under

            return true;
        }
    }
}
