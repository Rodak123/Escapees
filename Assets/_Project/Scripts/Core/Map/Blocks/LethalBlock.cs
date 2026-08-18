using UnityEngine;

namespace GameJam
{
    public class LethalBlock : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.TryGetComponent(out Lebro lebro))
                return;
            lebro.Die();
        }
    }
}
