using UnityEngine;

namespace GameJam
{
    public class ToolBeltPickuper : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] private GameObject spritesContainer;
        [SerializeField] private SpriteRenderer pickupSprite;

        private ToolBelt toolBelt;

        private void Awake()
        {
            pickupSprite.enabled = false;
        }

        private void Start()
        {
            toolBelt = GameContext.Instance.ToolBelt;
        }

        private void Update()
        {
            spritesContainer.SetActive(toolBelt.IsPickingUpAllowed);
            transform.position = toolBelt.WorldInteractPosition;

            pickupSprite.enabled = toolBelt.IsHoveringPickupableTool;
        }
    }
}
