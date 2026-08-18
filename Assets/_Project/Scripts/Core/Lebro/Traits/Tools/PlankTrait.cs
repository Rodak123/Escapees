using UnityEngine;

namespace GameJam
{
    public class PlankTrait : AToolTrait
    {
        public enum BuildDirection
        {
            Left,
            Right,
        }

        [Header("Visual")]
        [SerializeField] private Sprite plankSprite;

        [Space]
        [SerializeField] private GameObject lebroPlankLeftSpritePrefab;
        [SerializeField] private GameObject lebroPlankRightSpritePrefab;
        [SerializeField] private GameObject lebroPlankWalkLeftSpritePrefab;
        [SerializeField] private GameObject lebroPlankWalkRightSpritePrefab;

        [Header("Config")]
        [SerializeField] private MapTile buildTileLeft;
        [SerializeField] private MapTile buildTileRight;
        [SerializeField] private float buildDuration = 1;

        private BuildDirection buildDirection = BuildDirection.Right;

        private Map map;
        private float buildTimer;

        private Vector2Int cellPosition;

        private Vector2 TargetBuildingPosition => map.CellToWorld(cellPosition) + new Vector2(buildDirection == BuildDirection.Left ? 0 : 2, 0);
        public Vector2Int BuildDirectionVector => buildDirection == BuildDirection.Left ? Vector2Int.left : Vector2Int.right;
        public LebroController.MovementGoal MoveDirection => buildDirection == BuildDirection.Left ? LebroController.MovementGoal.WalkLeft : LebroController.MovementGoal.WalkRight;
        public MapTile BuildTile => buildDirection == BuildDirection.Left ? buildTileLeft : buildTileRight;

        private void ChangeCustomVisual(bool enable)
        {
            LebroVisual.SpriteType moveSpriteType = buildDirection == BuildDirection.Left
                ? LebroVisual.SpriteType.RunningLeft
                : LebroVisual.SpriteType.RunningRight;
            GameObject moveSprite = buildDirection == BuildDirection.Left
                ? lebroPlankWalkLeftSpritePrefab
                : lebroPlankWalkRightSpritePrefab;

            LebroVisual.SpriteType buildSpriteType = LebroVisual.SpriteType.Standing;
            GameObject buildSprite = buildDirection == BuildDirection.Left
                ? lebroPlankLeftSpritePrefab
                : lebroPlankRightSpritePrefab;

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

        private bool CanBuildFrom(Map map, Vector2Int cellPosition, bool isPlacing)
        {
            Vector2Int above = cellPosition + Vector2Int.up;
            Vector2Int ahead = cellPosition + BuildDirectionVector;
            Vector2Int behind = cellPosition - BuildDirectionVector;
            Vector2Int aheadAbove = cellPosition + BuildDirectionVector + Vector2Int.up;
            Vector2Int behindAbove = cellPosition - BuildDirectionVector + Vector2Int.up;

            return
                map.TryGetTileAt(above, out MapTile tileAbove)
                && tileAbove == null // needs empty space above to walk up
                && map.TryGetTileAt(behind, out MapTile tileBehind)
                && tileBehind == null // needs empty space behind to stand
                && (isPlacing ||
                    map.TryGetTileAt(aheadAbove, out MapTile tileAheadAbove)
                    && tileAheadAbove == null
                ) // needs space to build to, except when placing than just 1 plank is ok
                && (
                    isPlacing ||
                    map.TryGetTileAt(ahead, out MapTile tileAhead)
                    && tileAhead == null
                ) // needs empty space, except when placing 
                && (
                    !isPlacing ||
                    map.TryGetTileAt(behindAbove, out MapTile tileBehindAbove)
                    && tileBehindAbove == null // needs to hit their heads, only when placing
                )
            ;
        }

        private bool BuildAndTryContinue()
        {
            Vector2Int ahead = cellPosition + BuildDirectionVector;

            cellPosition += BuildDirectionVector + Vector2Int.up;

            if (!map.TryBuildTileAt(ahead, BuildTile))
                return false;

            return CanBuildFrom(map, cellPosition, false);
        }

        protected override void UpdateToolTrait()
        {
            Debug.DrawLine(Lebro.Controller.transform.position, map.CellToWorld(cellPosition), Color.green);
            Debug.DrawLine(Lebro.Controller.transform.position, TargetBuildingPosition);

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

                // this is to fix that these dumb ahh escapees keep getting stuck under the platform
                transform.position = transform.position - new Vector3(BuildDirectionVector.x, 1) * 0.01f;
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

            Lebro.Controller.OnHitWall -= LebroController_OnHitWall;
        }

        public override void Apply(Vector2 obtainPosition)
        {
            map = GameContext.Instance.Map;

            ChangeCustomVisual(true);

            cellPosition = map.WorldToCell(obtainPosition) - BuildDirectionVector;
            Lebro.Controller.transform.position = TargetBuildingPosition;

            Lebro.Controller.OnHitWall += LebroController_OnHitWall;
        }

        private void LebroController_OnHitWall(Lebro lebro)
        {
            CancelTrait(0);
        }

        protected override SpriteRendererConfig GetIconConfig()
        {
            return new()
            {
                Sprite = plankSprite,
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
            Vector2Int behindBelow = cellPosition - BuildDirectionVector + Vector2Int.down;

            if (!map.TryGetTileAt(behindBelow, out MapTile tileBehindBelow) || tileBehindBelow == null)
                return false; // no tile to go from onto this one

            return CanBuildFrom(map, cellPosition, true);
        }
    }
}
