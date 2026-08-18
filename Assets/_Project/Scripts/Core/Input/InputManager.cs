using Rodak.Utils.Singleton;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameJam
{
    [DefaultExecutionOrder(-100)]
    public class InputManager : SingletonMonoBehaviour<InputManager>
    {
        private InputSystem_Actions actions;

        public InputSystem_Actions.UIActions UIActions => actions.UI;
        public InputSystem_Actions.GameActions GameActions => actions.Game;
        public InputSystem_Actions.PlayerActions PlayerActions => actions.Player;
        public InputSystem_Actions.CameraActions CameraActions => actions.Camera;

        protected override void Awake()
        {
            actions = new();
        }

        private void OnEnable()
        {
            actions.Enable();

            actions.UI.Enable();
            actions.Game.Enable();
            actions.Player.Enable();
            actions.Camera.Enable();
        }

        private void OnDisable()
        {
            actions.Disable();

            actions.UI.Disable();
            actions.Game.Disable();
            actions.Player.Disable();
            actions.Camera.Disable();
        }

        // UI

        public InputAction UIClickAction => actions.UI.Click;

        public Vector2 ReadMousePosition()
        {
            return actions.UI.Point.ReadValue<Vector2>();
        }

        public Vector2 ReadMouseWorldPosition()
        {
            return Camera.main.ScreenToWorldPoint(ReadMousePosition());
        }

        public float ReadUIScrollWheel()
        {
            return actions.UI.ScrollWheel.ReadValue<Vector2>().y;
        }

        // App

        public bool WasApplicationQuitPressedThisFrame()
        {
            return actions.Application.Quit.WasPressedThisFrame();
        }

        // Game

        public bool WasGameBackPressedThisFrame()
        {
            return actions.Game.Back.WasPressedThisFrame();
        }

        public bool WasGameQuickRestartPressedThisFrame()
        {
            return actions.Game.QuickRestart.WasPressedThisFrame();
        }

        public bool WasGameNextLevelPressedThisFrame()
        {
            return actions.Game.NextLevel.WasPressedThisFrame();
        }

        public bool WasGameTogglePausePressedThisFrame()
        {
            return actions.Game.TogglePause.WasPressedThisFrame();
        }

        // Player

        public bool WasPlayerInteractPrimaryPressedThisFrame()
        {
            return actions.Player.Interact.WasPressedThisFrame();
        }

        public bool WasPlayerInteractSecondaryPressedThisFrame()
        {
            return actions.Player.InteractSecondary.WasPressedThisFrame();
        }

        public int GetPlayerPickItemIndexThisFrame()
        {
            if (actions.Player.PickItemA.WasPressedThisFrame()) return 0;
            if (actions.Player.PickItemB.WasPressedThisFrame()) return 1;
            if (actions.Player.PickItemC.WasPressedThisFrame()) return 2;
            if (actions.Player.PickItemD.WasPressedThisFrame()) return 3;
            if (actions.Player.PickItemE.WasPressedThisFrame()) return 4;
            if (actions.Player.PickItemF.WasPressedThisFrame()) return 5;
            if (actions.Player.PickItemG.WasPressedThisFrame()) return 6;
            if (actions.Player.PickItemH.WasPressedThisFrame()) return 7;
            if (actions.Player.PickItemI.WasPressedThisFrame()) return 8;
            if (actions.Player.PickItemJ.WasPressedThisFrame()) return 9;
            return -1;
        }

        public int GetPlayerScrollPickItemDeltaThisFrame()
        {
            float scroll = actions.Player.ScrollPickItemDelta.ReadValue<float>();

            if (scroll < 0) return -1;
            if (scroll > 0) return 1;
            return 0;
        }

        public bool WasPlayerChangeToolModePressedThisFrame()
        {
            return actions.Player.ChangeToolMode.WasPressedThisFrame();
        }

        // Camera

        public bool WasCameraPanDraggingPressedThisFrame()
        {
            return actions.Camera.PanDragging.WasPressedThisFrame();
        }

        public Vector2 ReadCameraPanDragPosition()
        {
            return actions.Camera.PanDragPosition.ReadValue<Vector2>();
        }

        public Vector2 ReadCameraMoveDelta()
        {
            return actions.Camera.MoveDelta.ReadValue<Vector2>();
        }

    }
}
