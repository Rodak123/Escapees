using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    public class ToolBelt : MonoBehaviour
    {
        [Header("Tools")]
        [SerializeField] private List<ToolSO> availableTools;
        [SerializeField] private Transform toolContainer;

        [SerializeField] private ToolTraitGiver toolTraitGiverPrefab;

        [Header("Settings")]
        [SerializeField] public bool IsInteractingEnabled = true;

        public bool IsPlacingAllowed => IsInteractingEnabled && !IsHoveringPickupableTool;
        public bool IsPickingUpAllowed => IsInteractingEnabled;

        // [field: SerializeField]
        public int ToolMode { get; private set; } = 0;
        public IReadOnlyList<ToolSO> AvailableTools => availableTools.AsReadOnly();
        public ToolSO SelectedTool => selectedIndex == -1 ? null : availableTools[selectedIndex];
        public bool CanPlaceSelectedTool => CanPlaceToolAt(InteractCellPosition, SelectedTool);

        public bool IsHoveringPickupableTool => placedTools.TryGetValue(InteractCellPosition, out ToolTraitGiver _);

        public Vector2Int InteractCellPosition { get; private set; }
        public Vector2 WorldInteractPosition => map.CellToWorld(InteractCellPosition);

        public event Action<int> OnToolSelectChanged;
        public event Action<ToolSO> OnToolPlaced;
        public event Action<ToolSO> OnToolPickedUp;

        private int selectedIndex = -1;

        private readonly Dictionary<Vector2Int, ToolTraitGiver> placedTools = new();

        private Map map;

        private void Awake()
        {
            map = GameContext.Instance.Map;
        }

        private void Update()
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(InputManager.Instance.ReadMousePosition());
            InteractCellPosition = map.WorldToCell(worldPosition);

            UpdateShortcuts();
            UpdatePickingUp();
            UpdatePlacing();
        }

        private void UpdateShortcuts()
        {
            if (InputManager.Instance.WasPlayerChangeToolModePressedThisFrame())
            {
                if (SelectedTool != null)
                {
                    ToolMode = SelectedTool.ToolTraitPrefab.GetNextSettingsStateIndex(ToolMode);
                    SelectedTool.ToolTraitPrefab.LoadSettingsState(ToolMode);
                }
            }

            int pickIndex = InputManager.Instance.GetPlayerPickItemIndexThisFrame();
            if (pickIndex != -1) SelectIndex(pickIndex);
        }

        private void UpdatePlacing()
        {
            if (!IsPlacingAllowed)
                return;

            if (!InputManager.Instance.WasPlayerInteractPrimaryPressedThisFrame())
                return;

            if (selectedIndex == -1)
                return;

            if (!CanPlaceToolAt(InteractCellPosition, availableTools[selectedIndex]))
                return;

            ToolSO tool = availableTools[selectedIndex];
            int toolMode = ToolMode;

            availableTools.RemoveAt(selectedIndex);
            DeselectIndex();

            // place the tool

            ToolTraitGiver toolTraitGiver = Instantiate(toolTraitGiverPrefab, toolContainer);
            toolTraitGiver.transform.position = WorldInteractPosition;
            toolTraitGiver.Init(tool, map, InteractCellPosition, toolMode);

            placedTools.Add(InteractCellPosition, toolTraitGiver);
            toolTraitGiver.OnConsumed += ToolTraitGiver_OnConsumed;

            OnToolPlaced?.Invoke(tool);
        }

        private void ToolTraitGiver_OnConsumed(ToolTraitGiver toolTraitGiver)
        {
            toolTraitGiver.OnConsumed -= ToolTraitGiver_OnConsumed;
            placedTools.Remove(toolTraitGiver.CellPosition);
        }

        private void UpdatePickingUp()
        {
            if (!IsPickingUpAllowed)
                return;

            if (!InputManager.Instance.WasPlayerInteractPrimaryPressedThisFrame())
                return;

            if (!placedTools.TryGetValue(InteractCellPosition, out ToolTraitGiver pickedUpTool))
                return;

            placedTools.Remove(InteractCellPosition);
            OnToolPickedUp?.Invoke(pickedUpTool.Tool); // before removing instance

            pickedUpTool.ReturnTool();

        }

        private bool CanPlaceToolAt(Vector2Int cellPosition, ToolSO tool)
        {
            if (tool == null)
                return false;

            if (!map.TryGetTileAt(cellPosition, out MapTile tile))
                return false;

            if (tile != null)
                return false;

            if (placedTools.TryGetValue(cellPosition, out ToolTraitGiver _))
                return false; // tool already there

            if (!tool.ToolTraitPrefab.CanBePlacedAt(map, cellPosition))
                return false;

            return true;
        }

        public void SelectIndex(int index)
        {
            if (index == -1 || index >= availableTools.Count)
            {
                return;
            }

            if (selectedIndex == index)
            {
                DeselectIndex();
                return;
            }

            if (selectedIndex != -1)
            {
                DeselectIndex();
            }

            selectedIndex = index;
            SelectedTool.ToolTraitPrefab.LoadSettingsState(ToolMode);
            OnToolSelectChanged?.Invoke(selectedIndex);
        }

        public void DeselectIndex()
        {
            if (selectedIndex == -1) return;
            selectedIndex = -1;
            OnToolSelectChanged?.Invoke(-1);
        }

        public void AddTool(ToolSO tool)
        {
            availableTools.Add(tool);
        }

        public void LoadTools(IEnumerable<ToolSO> tools)
        {
            availableTools.Clear();
            availableTools.AddRange(tools);
        }
    }
}
