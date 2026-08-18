using UnityEngine;

namespace GameJam
{
    public class ToolGhost : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] private GameObject spritesContainer;
        [SerializeField] private SpriteRenderer iconSprite;
        [SerializeField] private SpriteRenderer spotSprite;

        [Header("Colors")]
        [SerializeField] private Color validTint = Color.green;
        [SerializeField] private Color invalidTint = Color.red;

        private ToolBelt toolBelt;

        private void Awake()
        {
            iconSprite.enabled = false;
            spotSprite.enabled = false;
        }

        private void Start()
        {
            toolBelt = GameContext.Instance.ToolBelt;
        }

        private void Update()
        {
            spritesContainer.SetActive(toolBelt.IsPlacingAllowed && !toolBelt.IsHoveringPickupableTool);

            transform.position = toolBelt.WorldInteractPosition;

            bool isToolSelected = toolBelt.SelectedTool != null;
            if (isToolSelected)
            {
                spotSprite.color = toolBelt.CanPlaceSelectedTool ? validTint : invalidTint;
            }

            if (toolBelt.SelectedTool == null)
            {
                iconSprite.enabled = false;
                spotSprite.enabled = false;
            }
            else
            {
                iconSprite.enabled = true;
                spotSprite.enabled = true;
                toolBelt.SelectedTool.ToolTraitPrefab.UpdateSpriteRenderer(iconSprite);
            }
        }
    }
}
