using UnityEngine;
using UnityEngine.UI;

namespace GameJam
{
    public class ToolPickButtonUI : ActionButtonUI
    {
        [SerializeField] private Image image;

        public void SetTool(ToolSO tool)
        {
            if (tool == null)
            {
                image.gameObject.SetActive(false);
                description = null;
            }
            else
            {
                image.gameObject.SetActive(true);
                description = tool.ToolDescription;
                image.sprite = tool.ToolIcon;
            }
        }

    }
}
