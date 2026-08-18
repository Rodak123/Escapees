using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameJam
{
    public class InteractionZone : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField, ReadOnly] private bool hovering;
        [SerializeField, ReadOnly] private bool triggering;

        public bool Hovering => hovering;
        public bool Triggering => triggering;

        public event Action<bool> OnHoverChanged;
        public event Action<bool> OnTriggerChanged;

        private void Start()
        {
            InputManager.Instance.UIClickAction.performed += InputManager_ClickPerformed;
        }

        private void OnDisable()
        {
            if (hovering) SetHover(false);
            if (triggering) SetTrigger(false);
        }

        private void OnMouseEnter()
        {
            SetHover(true);
        }

        private void OnMouseExit()
        {
            SetHover(false);
        }

        private void InputManager_ClickPerformed(InputAction.CallbackContext context)
        {
            bool isPressed = context.ReadValueAsButton();
            if (isPressed && Hovering)
            {
                SetTrigger(true);
            }
            else if (!isPressed)
            {
                SetTrigger(false);
            }
        }

        private void SetHover(bool value)
        {
            hovering = value;
            OnHoverChanged?.Invoke(value);
        }

        private void SetTrigger(bool value)
        {
            triggering = value;
            OnTriggerChanged?.Invoke(value);
        }
    }
}