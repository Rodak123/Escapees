using UnityEngine;

namespace GameJam
{
    public class ToolBeltGunCursor : MonoBehaviour
    {
        [SerializeField] private ToolBeltGun toolBeltGun;

        [Header("Visuals")]
        [SerializeField] private GameObject spritesContainer;
        [SerializeField] private SpriteRenderer cursorSprite;

        [Header("Colors")]
        [SerializeField] private Color aimingTint = Color.red;
        [SerializeField] private Color readyTint = Color.green;

        private void Update()
        {
            spritesContainer.SetActive(toolBeltGun.IsGunSelected);

            Lebro target = toolBeltGun.TargetedLebro;

            if (target == null)
            {
                transform.position = toolBeltGun.AimPosition;
                cursorSprite.color = aimingTint;
            }
            else
            {
                transform.position = target.transform.position + new Vector3(-1, 1);
                cursorSprite.color = readyTint;
            }
        }
    }
}
