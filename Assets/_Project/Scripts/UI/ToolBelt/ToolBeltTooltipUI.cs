using TMPro;
using UnityEngine;

namespace GameJam
{
    public class ToolBeltTooltipUI : MonoBehaviour
    {
        private enum AnimationState
        {
            Opening,
            Closing
        }

        [SerializeField] private ToolBeltUI toolBeltUI;

        [Header("UI")]
        [SerializeField] private RectTransform tooltipContainer;
        [SerializeField] private TMP_Text tooltipText;

        [Header("Animation")]
        [SerializeField] private float moveSpeed = 64f;

        private Vector2 closedPosition;
        private Vector2 openPosition;

        private string nextTooltip;

        private AnimationState animationState;

        private bool isClosed => Vector2.Distance(tooltipContainer.anchoredPosition, closedPosition) < 0.0001f;

        private void Awake()
        {
            float containerHeight = tooltipContainer.rect.height;

            openPosition = new Vector2(tooltipContainer.anchoredPosition.x, 0f);
            closedPosition = new Vector2(tooltipContainer.anchoredPosition.x, containerHeight + 1f);

            tooltipContainer.anchoredPosition = closedPosition;
            animationState = AnimationState.Closing;
        }

        private void Start()
        {
            foreach (ActionButtonUI button in toolBeltUI.ActionButtons)
            {
                button.OnMouseEntered += ActionButtonUI_OnMouseEntered;
                button.OnMouseExited += ActionButtonUI_OnMouseExited;
            }
        }

        private void Update()
        {
            switch (animationState)
            {
                case AnimationState.Opening:
                    tooltipContainer.anchoredPosition = Vector2.MoveTowards(
                        tooltipContainer.anchoredPosition,
                        openPosition,
                        moveSpeed * Time.deltaTime
                    );
                    break;
                case AnimationState.Closing:
                    tooltipContainer.anchoredPosition = Vector2.MoveTowards(
                        tooltipContainer.anchoredPosition,
                        closedPosition,
                        moveSpeed * Time.deltaTime
                    );
                    break;
            }

            // Debug.Log(animationState);
            // Debug.Log(tooltipContainer.anchoredPosition);
            // Debug.Log(openPosition);
            // Debug.Log(closedPosition);

            if (isClosed && nextTooltip != null)
            {
                animationState = AnimationState.Opening;

                tooltipText.text = nextTooltip;
                nextTooltip = null;
            }
        }

        private void ShowTooltip(string tooltip)
        {
            if (string.IsNullOrEmpty(tooltip)) return;
            nextTooltip = tooltip;
            if (!isClosed)
            {
                animationState = AnimationState.Closing;
            }
        }

        private void HideTooltip()
        {
            animationState = AnimationState.Closing;
            nextTooltip = null;
        }

        private void ActionButtonUI_OnMouseEntered(ActionButtonUI button)
        {
            ShowTooltip(button.Description);
        }

        private void ActionButtonUI_OnMouseExited(ActionButtonUI button)
        {
            HideTooltip();
        }
    }
}