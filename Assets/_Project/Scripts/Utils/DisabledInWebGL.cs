using UnityEngine;

namespace GameJam
{
    [DefaultExecutionOrder(-50)]
    public class DisabledInWebGL : MonoBehaviour
    {
#if UNITY_WEBGL
        private void Awake()
        {
            gameObject.SetActive(false);
        }
#endif
    }
}
