using UnityEngine;

namespace GameJam
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class LebroDestroyer : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.TryGetComponent(out Lebro lebro))
                return;

            GameContext.Instance.LebroManager.TryRemoveLebro(lebro);
        }
    }
}
