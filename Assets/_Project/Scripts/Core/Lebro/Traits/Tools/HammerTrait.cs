using UnityEngine;

namespace GameJam
{
    public class HammerTrait : AToolTrait
    {
        public enum BuildDirection
        {
            Left,
            Right,
        }

        [Header("Visual")]
        [SerializeField] private Sprite hammerSprite;

        [Space]
        [SerializeField] private GameObject lebroHammerLeftSpritePrefab;
        [SerializeField] private GameObject lebroHammerRightSpritePrefab;
        [SerializeField] private GameObject lebroHammerWalkLeftSpritePrefab;
        [SerializeField] private GameObject lebroHammerWalkRightSpritePrefab;

        [Header("Config")]
        [SerializeField] private MapTile buildTile;
        [SerializeField] private float buildDuration = 1;

        private BuildDirection buildDirection = BuildDirection.Right;

        private Map map;
        private float buildTimer;

        private Vector2Int cellPosition;

        private Vector2 TargetBuildingPosition => map.CellToWorld(cellPosition) + new Vector2(buildDirection == BuildDirection.Left ? 2 : 0, 0);
        public Vector2Int BuildDirectionVector => buildDirection == BuildDirection.Left ? Vector2Int.left : Vector2Int.right;
        public LebroController.MovementGoal MoveDirection => buildDirection == BuildDirection.Left ? LebroController.MovementGoal.WalkLeft : LebroController.MovementGoal.WalkRight;

        private void ChangeCustomVisual(bool enable)
        {
            LebroVisual.SpriteType moveSpriteType = buildDirection == BuildDirection.Left
                ? LebroVisual.SpriteType.RunningLeft
                : LebroVisual.SpriteType.RunningRight;
            GameObject moveSprite = buildDirection == BuildDirection.Left
                ? lebroHammerWalkLeftSpritePrefab
                : lebroHammerWalkRightSpritePrefab;

            LebroVisual.SpriteType buildSpriteType = LebroVisual.SpriteType.Standing;
            GameObject buildSprite = buildDirection == BuildDirection.Left
                ? lebroHammerLeftSpritePrefab
                : lebroHammerRightSpritePrefab;

            if (enable)
            {
                Lebro.Visual.EnableCustomVisualFor(moveSpriteType, moveSprite);
                Lebro.Visual.EnableCustomVisualFor(buildSpriteType, buildSprite);
            }
            else
            {
                Lebro.Visual.DisableCustomVisualFor(moveSpriteType);
                Lebro.Visual.DisableCustomVisualFor(buildSpriteType);
            }
        }

        private bool CanBuildFrom(Map map, Vector2Int cellPosition)
        {
            Vector2Int ahead = cellPosition + BuildDirectionVector;
            Vector2Int aheadBelow = cellPosition + BuildDirectionVector + Vector2Int.down;

            return map.TryGetTileAt(ahead, out MapTile tileAhead)
                && tileAhead == null
                && map.TryGetTileAt(aheadBelow, out MapTile tileAheadBelow)
                && (tileAheadBelow == null || !tileAheadBelow.IsFlat);
        }

        private bool BuildAndTryContinue()
        {
            Vector2Int aheadBelow = cellPosition + BuildDirectionVector + Vector2Int.down;

            cellPosition += BuildDirectionVector;

            if (!map.TryBuildTileAt(aheadBelow, buildTile))
                return false;

            return CanBuildFrom(map, cellPosition);
        }

        protected override void UpdateToolTrait()
        {
            if (!Lebro.Controller.IsGrounded)
            {
                CancelTrait(0);
                return;
            }

            if (Vector2.Distance(Lebro.Controller.transform.position, TargetBuildingPosition) > 1)
            {
                Lebro.Controller.CurrentMovementGoal = MoveDirection;
                return; // walk until we reach the wall
            }

            Lebro.Controller.transform.position = TargetBuildingPosition;
            Lebro.Controller.CurrentMovementGoal = LebroController.MovementGoal.Standing;

            buildTimer += Time.deltaTime;

            if (buildTimer >= buildDuration)
            {
                buildTimer = 0;
                bool continuing = BuildAndTryContinue();
                if (!continuing) CancelTrait(0);
            }
        }

        protected override int GetPlayerCancelToken()
        {
            if (InputManager.Instance.WasPlayerInteractSecondaryPressedThisFrame())
                return 0;
            return -1;
        }

        protected override void CancelTrait(int token)
        {
            base.CancelTrait(token);

            Lebro.Controller.CurrentMovementGoal = MoveDirection;

            ChangeCustomVisual(false);
        }

        public override void Apply(Vector2 obtainPosition)
        {
            map = GameContext.Instance.Map;

            ChangeCustomVisual(true);

            cellPosition = map.WorldToCell(obtainPosition);
            Lebro.Controller.transform.position = TargetBuildingPosition;

            Vector2Int below = cellPosition + Vector2Int.down;
            if (!map.TryGetTileAt(below, out MapTile tileBelow) || !tileBelow.IsFlat)
            {
                map.TryBuildTileAt(below, buildTile); // ensure standing on flat ground
            }
        }

        protected override SpriteRendererConfig GetIconConfig()
        {
            return new()
            {
                Sprite = hammerSprite,
                IsXFlipped = buildDirection == BuildDirection.Left,
            };
        }

        public override void LoadSettingsState(int index)
        {
            buildDirection = index % 2 == 0 ? BuildDirection.Right : BuildDirection.Left;
        }

        public override int GetNextSettingsStateIndex(int currentIndex)
        {
            return (currentIndex + 1) % 2;
        }

        public override bool CanBeGivenTo(Lebro lebro)
        {
            return lebro.Controller.IsGrounded;
        }

        public override bool CanBePlacedAt(Map map, Vector2Int cellPosition)
        {
            Vector2Int below = cellPosition + Vector2Int.down;

            if (!map.TryGetTileAt(below, out MapTile tileBelow) || tileBelow == null)
                return false; // no tile under

            return CanBuildFrom(map, cellPosition);
        }
    }
}
