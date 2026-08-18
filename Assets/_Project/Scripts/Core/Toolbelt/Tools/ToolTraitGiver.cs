using System;
using UnityEngine;

namespace GameJam
{
    public class ToolTraitGiver : MonoBehaviour
    {
        [SerializeField] private ToolSO defaultTool;
        [SerializeField] private int defaultSettingsStateIndex;
        [SerializeField] private bool disableCancellation;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        private int settingsStateIndex;
        private ToolSO tool;

        private bool isConsumed;

        private Map map;
        private Vector2Int cellPosition;

        public Vector2Int CellPosition => cellPosition;
        public ToolSO Tool => tool;

        private AToolTrait toolTraitInstance;

        public event Action<ToolTraitGiver> OnConsumed;

        private void Start()
        {
            if (tool != null || defaultTool == null)
                return;

            Map map = GameContext.Instance.Map;
            Init(defaultTool, map, map.WorldToCell(transform.position), defaultSettingsStateIndex);
        }

        private void Update()
        {
            if (tool == null)
                return;

            toolTraitInstance.UpdateSpriteRenderer(spriteRenderer);

            if (
                !map.TryGetTileAt(cellPosition, out MapTile tile)
                || tile != null // tool is inside a tile :O
                || !toolTraitInstance.CanBePlacedAt(map, cellPosition))
            {
                ReturnTool();
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (tool == null)
                return;

            if (isConsumed)
                return;

            if (!collision.gameObject.TryGetComponent(out Lebro lebro))
                return;

            if (!toolTraitInstance.CanBeGivenTo(lebro))
                return;

            if (!lebro.Personality.TryGiveTrait(tool.ToolTraitPrefab, out ALebroTrait trait))
                return;

            AToolTrait toolTrait = (AToolTrait)trait;
            if (disableCancellation) toolTrait.IsCancellationEnabled = false;
            toolTrait.LoadSettingsState(settingsStateIndex);
            toolTrait.Apply(transform.position);

            if (tool.IsSingleUse)
            {
                Consume();
            }
        }

        private void Consume()
        {
            if (isConsumed) return;
            isConsumed = true;
            OnConsumed?.Invoke(this);
            Destroy(gameObject);
        }

        public void Init(ToolSO tool, Map map, Vector2Int cellPosition, int settingsStateIndex)
        {
            this.map = map;
            this.cellPosition = cellPosition;
            this.tool = tool;
            this.settingsStateIndex = settingsStateIndex;

            toolTraitInstance = Instantiate(tool.ToolTraitPrefab, transform);
            toolTraitInstance.gameObject.SetActive(false);

            toolTraitInstance.LoadSettingsState(settingsStateIndex);
        }

        public void ReturnTool()
        {
            if (isConsumed) return;
            Consume();
            GameContext.Instance.ToolBelt?.AddTool(tool);
        }
    }
}
