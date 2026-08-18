using UnityEngine;

namespace GameJam
{
    public class LebroEnd : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private DestroyAfter lebroEndAnimation;

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!collision.gameObject.TryGetComponent(out Lebro lebro))
                return;

            if (!lebro.Controller.IsGrounded)
                return; // has to be on the ground

            if (lebro.Personality.CurrentTrait != null)
                return; // to not rob the player of a tool :P

            EndLebro(lebro);
        }

        private void EndLebro(Lebro lebro)
        {
            DestroyAfter endAnimation = Instantiate(lebroEndAnimation, transform);
            endAnimation.gameObject.SetActive(true);

            GameContext.Instance.LebroManager.TryRemoveLebro(lebro);
        }
    }
}
