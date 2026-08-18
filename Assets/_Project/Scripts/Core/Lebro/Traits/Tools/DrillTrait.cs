using UnityEngine;

namespace GameJam
{
    public class DrillTrait : AToolTrait
    {
        public enum MineDirection
        {
            Left,
            Right,
        }

        [Header("Visual")]
        [SerializeField] private Sprite drillSprite;

        [Space]
        [SerializeField] private GameObject lebroDrillLeftSpritePrefab;
        [SerializeField] private GameObject lebroDrillRightSpritePrefab;
        [SerializeField] private GameObject lebroDrillWalkLeftSpritePrefab;
        [SerializeField] private GameObject lebroDrillWalkRightSpritePrefab;

        [Header("Sounds")]
        [SerializeField] private SoundEffect drillSound;
        [SerializeField] private float drillSoundInterval = 1.2f;

        [Header("Config")]
        [SerializeField] private float mineDuration = 1.2f;

        private MineDirection mineDirection = MineDirection.Right;

        private Map map;
        private float mineTimer;

        private TimedRepeater drillSoundRepeater;

        private Vector2Int cellPosition;

        private Vector2 TargetMiningPosition => map.CellToWorld(cellPosition) + new Vector2(mineDirection == MineDirection.Left ? 2 : 0, 0);
        public Vector2Int MineDirectionVector => mineDirection == MineDirection.Left ? Vector2Int.left : Vector2Int.right;
        public LebroController.MovementGoal MoveDirection => mineDirection == MineDirection.Left ? LebroController.MovementGoal.WalkLeft : LebroController.MovementGoal.WalkRight;

        private void ChangeCustomVisual(bool enable)
        {
            LebroVisual.SpriteType moveSpriteType = mineDirection == MineDirection.Left
                ? LebroVisual.SpriteType.RunningLeft
                : LebroVisual.SpriteType.RunningRight;
            GameObject moveSprite = mineDirection == MineDirection.Left
                ? lebroDrillWalkLeftSpritePrefab
                : lebroDrillWalkRightSpritePrefab;

            LebroVisual.SpriteType mineSpriteType = LebroVisual.SpriteType.Standing;
            GameObject mineSprite = mineDirection == MineDirection.Left
                ? lebroDrillLeftSpritePrefab
                : lebroDrillRightSpritePrefab;

            if (enable)
            {
                Lebro.Visual.EnableCustomVisualFor(moveSpriteType, moveSprite);
                Lebro.Visual.EnableCustomVisualFor(mineSpriteType, mineSprite);
            }
            else
            {
                Lebro.Visual.DisableCustomVisualFor(moveSpriteType);
                Lebro.Visual.DisableCustomVisualFor(mineSpriteType);
            }
        }

        private bool CanMineFrom(Map map, Vector2Int cellPosition)
        {
            Vector2Int ahead = cellPosition + MineDirectionVector;

            if (
               !map.TryGetTileAt(ahead, out MapTile tileAhead)
               || tileAhead == null
               || !tileAhead.Destroyable
           ) return false; // no destroyable wall ahead

            return true;
        }

        private bool MineAndTryContinue()
        {
            Vector2Int ahead = cellPosition + MineDirectionVector;

            cellPosition += MineDirectionVector;

            if (!map.TryDestroyTileAt(ahead))
                return false;

            return CanMineFrom(map, cellPosition);
        }

        protected override void UpdateToolTrait()
        {
            if (!Lebro.Controller.IsGrounded)
            {
                CancelTrait(0);
                return;
            }

            if (Vector2.Distance(Lebro.Controller.transform.position, TargetMiningPosition) > 1)
            {
                Lebro.Controller.CurrentMovementGoal = MoveDirection;
                return; // walk until we reach the wall
            }

            Lebro.Controller.transform.position = TargetMiningPosition;
            Lebro.Controller.CurrentMovementGoal = LebroController.MovementGoal.Standing;

            mineTimer += Time.deltaTime;
            drillSoundRepeater.Update(Time.deltaTime);

            if (mineTimer >= mineDuration)
            {
                mineTimer = 0;
                bool continuing = MineAndTryContinue();
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
            drillSoundRepeater = new(drillSoundInterval, () => Lebro.Sounds.PlaySFX(drillSound));

            map = GameContext.Instance.Map;

            ChangeCustomVisual(true);

            cellPosition = map.WorldToCell(obtainPosition);
            Lebro.Controller.transform.position = TargetMiningPosition;
        }

        protected override SpriteRendererConfig GetIconConfig()
        {
            return new()
            {
                Sprite = drillSprite,
                IsXFlipped = mineDirection == MineDirection.Left,
            };
        }

        public override void LoadSettingsState(int index)
        {
            mineDirection = index % 2 == 0 ? MineDirection.Right : MineDirection.Left;
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

            if (!map.TryGetTileAt(below, out MapTile tileBelow) || tileBelow == null || !tileBelow.IsFlat)
                return false; // no tile is under

            return CanMineFrom(map, cellPosition);
        }
    }
}
