using UnityEngine;
using TMPro;

namespace GameJam
{
    public class ApplicationVersionText : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        private void Awake()
        {
            text.text = Application.version;
        }
    }
}
