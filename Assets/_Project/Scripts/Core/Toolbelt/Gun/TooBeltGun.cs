using System;
using UnityEngine;

namespace GameJam
{
    public class ToolBeltGun : MonoBehaviour
    {
        [SerializeField] private ToolBelt toolBelt;

        [Header("Lebro Visual")]
        [SerializeField] private GameObject lebroDeathSprite;

        private LebroManager lebroManager;

        private bool isGunSelected;

        public event Action<bool> OnGunSelectChanged;

        public bool IsGunSelected => isGunSelected;

        public Vector2 AimPosition => toolBelt.WorldInteractPosition;
        public Lebro TargetedLebro => lebroManager.HoveredLebro;


        private void Awake()
        {
            lebroManager = GameContext.Instance.LebroManager;

            toolBelt.OnToolSelectChanged += ToolBelt_OnToolSelectChanged;
        }

        private void Update()
        {
            UpdateShooting();
            UpdateShortcuts();
        }

        private void UpdateShooting()
        {
            if (!IsGunSelected)
                return;

            if (TargetedLebro == null)
                return;

            if (!InputManager.Instance.WasPlayerInteractPrimaryPressedThisFrame())
                return;

            Lebro victim = TargetedLebro;

            victim.Visual.EnableCustomVisualFor(LebroVisual.SpriteType.Death, lebroDeathSprite);
            victim.Die();
        }

        private void UpdateShortcuts()
        {
            int pickIndex = InputManager.Instance.GetPlayerPickItemIndexThisFrame();
            if (pickIndex == 9) SelectGun();
        }

        private void ToolBelt_OnToolSelectChanged(int index)
        {
            if (index == -1) return;
            DeselectGun();
        }

        public void SelectGun()
        {
            if (isGunSelected) return;
            isGunSelected = true;
            OnGunSelectChanged?.Invoke(true);
            toolBelt.DeselectIndex();
        }

        public void DeselectGun()
        {
            if (!isGunSelected) return;
            isGunSelected = false;
            OnGunSelectChanged?.Invoke(false);
        }
    }
}
