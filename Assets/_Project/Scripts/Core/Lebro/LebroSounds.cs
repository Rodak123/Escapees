using UnityEngine;

namespace GameJam
{
    [RequireComponent(typeof(Lebro))]
    public class LebroSounds : MonoBehaviour
    {
        [Header("Lebro Sounds")]
        [SerializeField] private SoundEffect deathSound;

        [Header("Lebro Controller Sounds")]
        [SerializeField] private SoundEffect stepSound;
        [SerializeField] private float stepSoundInterval = 0.2f;
        [SerializeField] private SoundEffect hitWallSound;
        [SerializeField] private SoundEffect landSound;

        private Lebro lebro;

        private TimedRepeater stepSoundRepeater;


        private void Awake()
        {
            lebro = GetComponent<Lebro>();

            lebro.OnDeath += Lebro_OnDeath;

            lebro.Controller.OnLanded += LebroController_OnLanded;
            lebro.Controller.OnHitWall += LebroController_OnHitWall;

            stepSoundRepeater = new(stepSoundInterval, () => PlaySFX(stepSound));
        }

        private void Update()
        {
            UpdateLebroSteps();
        }

        private void UpdateLebroSteps()
        {
            if (lebro.Controller.CurrentMovementGoal == LebroController.MovementGoal.Standing || !lebro.Controller.IsGrounded)
                return;

            stepSoundRepeater.Update(Time.deltaTime);
        }

        private void LebroController_OnHitWall(Lebro lebro)
        {
            PlaySFX(hitWallSound);
        }

        private void LebroController_OnLanded(Lebro lebro)
        {
            PlaySFX(landSound);
        }

        private void Lebro_OnDeath(Lebro lebro)
        {
            PlaySFX(deathSound);
        }

        public void PlaySFX(SoundEffect soundEffect, float volumeScale = 1f)
        {
            SFXManager.Instance.PlaySFX(soundEffect, volumeScale);
        }
    }
}
