using UnityEngine;

namespace GameJam
{
    public class LebroTraitSounds : MonoBehaviour
    {
        [SerializeField] private AToolTrait toolTrait;

        [Header("Sounds")]
        [SerializeField] private SoundEffect cancelledSound;

        private void Awake()
        {
            toolTrait.OnCancelledByPlayer += AToolTrait_OnCancelledByPlayer;
        }

        private void AToolTrait_OnCancelledByPlayer()
        {
            toolTrait.Lebro.Sounds.PlaySFX(cancelledSound);
        }
    }
}
