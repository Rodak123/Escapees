using UnityEngine;

namespace GameJam
{
    public class SFXPlayer : MonoBehaviour
    {
        [SerializeField] private SoundEffect soundEffect;

        public void Play()
        {
            PlayAt(1f);
        }

        public void PlayAt(float volumeScale)
        {
            SFXManager.Instance.PlaySFX(soundEffect, volumeScale);
        }
    }
}
