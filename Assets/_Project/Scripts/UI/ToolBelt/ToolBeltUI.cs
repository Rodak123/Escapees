using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    public class ToolBeltUI : MonoBehaviour
    {
        [SerializeField] private ToolBelt toolBelt;
        [SerializeField] private ToolBeltGun toolBeltGun;

        [Header("UI")]
        [SerializeField] private List<ToolPickButtonUI> toolPickButtons;
        [SerializeField] private ToolBeltGunActionButtonUI gunButton;

        private readonly List<ActionButtonUI> actionButtons = new();
        private ActionButtonUI selectedButton;

        public ToolBelt ToolBelt => toolBelt;
        public IReadOnlyList<ActionButtonUI> ActionButtons => actionButtons;

        private void Awake()
        {
            actionButtons.AddRange(toolPickButtons);
            actionButtons.Add(gunButton);

            for (int i = 0; i < ActionButtons.Count; i++)
            {
                ActionButtonUI button = ActionButtons[i];
                button.OnClick += ActionButtonUI_OnClick;
            }

            gunButton.OnClick += (ActionButtonUI button) =>
            {
                if (toolBeltGun.IsGunSelected)
                {
                    toolBeltGun.DeselectGun();
                }
                else
                {
                    toolBelt.DeselectIndex();
                    toolBeltGun.SelectGun();
                }
            };

            toolBelt.OnToolSelectChanged += ToolBelt_OnToolSelectChanged;
            toolBeltGun.OnGunSelectChanged += ToolBeltGun_OnGunSelectChanged;
        }

        private void Update()
        {
            for (int i = 0; i < toolPickButtons.Count; i++)
            {
                ToolPickButtonUI button = toolPickButtons[i];
                if (i >= toolBelt.AvailableTools.Count)
                {
                    button.SetTool(null);
                }
                else
                {
                    button.SetTool(toolBelt.AvailableTools[i]);
                }
            }
        }

        private void ToolBeltGun_OnGunSelectChanged(bool isGunSelected)
        {
            if (isGunSelected)
            {
                gunButton.Select();
            }
            else
            {
                gunButton.Unselect();
            }
        }

        private void ToolBelt_OnToolSelectChanged(int index)
        {
            if (selectedButton != null)
            {
                selectedButton.Unselect();
                selectedButton = null;
            }

            if (index < 0 || index >= toolPickButtons.Count)
                return;

            selectedButton = toolPickButtons[index];
            selectedButton.Select();
        }

        private void ActionButtonUI_OnClick(ActionButtonUI button)
        {
            int index = -1;
            for (int i = 0; i < toolPickButtons.Count; i++)
            {
                if (button == toolPickButtons[i])
                {
                    index = i;
                    break;
                }
            }

            toolBelt.SelectIndex(index);
        }
    }
}
