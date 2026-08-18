using System;
using UnityEngine;

namespace GameJam
{
    public class Lebro : MonoBehaviour
    {
        [Header("Systems")]
        [SerializeField] private LebroController controller;
        [SerializeField] private LebroVisual visual;
        [SerializeField] private LebroPersonality personality;
        [SerializeField] private LebroSounds sounds;

        private bool isDead;
        private bool isHovered;
        private bool isPaused;

        public LebroController Controller => controller;
        public LebroVisual Visual => visual;
        public LebroPersonality Personality => personality;
        public LebroSounds Sounds => sounds;

        public bool IsDead => isDead;
        public bool IsHovered => isHovered;
        public bool IsPaused => isPaused;

        public event Action<Lebro> OnDeath;
        public event Action<Lebro> OnHoveredChanged;

        private void OnMouseEnter()
        {
            isHovered = true;
            OnHoveredChanged?.Invoke(this);
        }

        private void OnMouseExit()
        {
            isHovered = false;
            OnHoveredChanged?.Invoke(this);
        }

        private void Start()
        {
            isDead = false;
        }

        public void Die()
        {
            if (isDead) return;
            isDead = true;
            OnDeath?.Invoke(this);

            DestroySelf();
        }

        public void Pause()
        {
            isPaused = true;
        }

        public void Unpause()
        {
            isPaused = false;
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}
