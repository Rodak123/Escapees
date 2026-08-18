using UnityEngine;

namespace GameJam
{
    public class PickaxeTrait : AToolTrait
    {
        [Header("Visual")]
        [SerializeField] private Sprite pickaxeSprite;

        [Space]
        [SerializeField] private GameObject lebroPickaxeSpritePrefab;
        [SerializeField] private GameObject lebroPickaxeFallSpritePrefab;

        [Header("Config")]
        [SerializeField] private float mineDuration = 2.5f;

        private Map map;
        private float mineTimer;

        private LebroController.MovementGoal previousMovementGoal;
        private Vector2Int cellPosition;

        private void ChangeCustomVisual(bool enable)
        {
            if (enable)
            {
                Lebro.Visual.EnableCustomVisualFor(LebroVisual.SpriteType.Standing, lebroPickaxeSpritePrefab);
                Lebro.Visual.EnableCustomVisualFor(LebroVisual.SpriteType.Falling, lebroPickaxeFallSpritePrefab);
            }
            else
            {
                Lebro.Visual.DisableCustomVisualFor(LebroVisual.SpriteType.Standing);
                Lebro.Visual.DisableCustomVisualFor(LebroVisual.SpriteType.Falling);
            }
        }

        private bool CanMineFrom(Map map, Vector2Int cellPosition)
        {
            Vector2Int below = cellPosition + Vector2Int.down;

            if (!map.TryGetTileAt(below, out MapTile tileBelow) || tileBelow == null)
                return false; // no tile is under

            if (!tileBelow.Destroyable)
                return false; // tile below is not destroyable

            return true;
        }

        private bool MineAndTryContinue()
        {
            Vector2Int below = cellPosition + Vector2Int.down;

            cellPosition += Vector2Int.down;

            if (!map.TryDestroyTileAt(below))
                return false;

            return CanMineFrom(map, cellPosition);
        }

        protected override int GetPlayerCancelToken()
        {
            if (InputManager.Instance.WasPlayerInteractSecondaryPressedThisFrame())
                return 0;
            return -1;
        }

        public override void Apply(Vector2 obtainPosition)
        {
            map = GameContext.Instance.Map;

            ChangeCustomVisual(true);

            previousMovementGoal = Lebro.Controller.CurrentMovementGoal;
            Lebro.Controller.CurrentMovementGoal = LebroController.MovementGoal.Standing;

            cellPosition = map.WorldToCell(obtainPosition);
            Vector2 worldPosition = map.CellToWorld(cellPosition);
            Lebro.Controller.transform.position = worldPosition;
        }

        protected override void UpdateToolTrait()
        {
            if (!Lebro.Controller.IsGrounded)
                return; // wait for ground

            mineTimer += Time.deltaTime;

            if (mineTimer >= mineDuration)
            {
                mineTimer = 0;
                bool continuing = MineAndTryContinue();
                if (!continuing) CancelTrait(0);
            }
        }

        protected override void CancelTrait(int token)
        {
            base.CancelTrait(token);

            ChangeCustomVisual(false);

            Lebro.Controller.CurrentMovementGoal = previousMovementGoal;
        }

        protected override SpriteRendererConfig GetIconConfig()
        {
            return new()
            {
                Sprite = pickaxeSprite,
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
            return CanMineFrom(map, cellPosition);
        }
    }
}
