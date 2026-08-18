using UnityEngine;

namespace GameJam
{
    [CreateAssetMenu(fileName = "Tool", menuName = "Tool")]
    public class ToolSO : ScriptableObject
    {
        [Header("Info")]
        [SerializeField] private string toolName = "Tool";
        [SerializeField] private string toolDescription = "Tool is Cool";

        [Header("Tool")]
        [SerializeField] private Sprite toolIcon;
        [SerializeField] private AToolTrait toolTraitPrefab;
        [SerializeField] private bool isSingleUse = true;

        public string ToolName => toolName;
        public string ToolDescription => toolDescription;

        public Sprite ToolIcon => toolIcon;
        public AToolTrait ToolTraitPrefab => toolTraitPrefab;
        public bool IsSingleUse => isSingleUse;

        public override string ToString()
        {
            return $"{toolName} - {toolDescription}";
        }
    }
}