using System;
using UnityEngine;

namespace GameJam
{
    public abstract class AToolTrait : ALebroTrait
    {
        protected struct SpriteRendererConfig
        {
            public Sprite Sprite;
            public bool IsXFlipped;
        }

        [Header("Cancellation")]
        [SerializeField] public bool IsCancellationEnabled;
        [SerializeField] private GameObject cancelSprite;

        public bool IsCanceled { get; private set; }

        public event Action OnCancelledByPlayer;

        public void UpdateSpriteRenderer(SpriteRenderer spriteRenderer)
        {
            if (spriteRenderer == null)
                throw new ArgumentNullException($"{nameof(spriteRenderer)} is null.");

            SpriteRendererConfig config = GetIconConfig();

            spriteRenderer.sprite = config.Sprite;
            spriteRenderer.flipX = config.IsXFlipped;

            spriteRenderer.transform.localPosition = new Vector2(
                config.IsXFlipped ? spriteRenderer.sprite.rect.width : 0,
                spriteRenderer.transform.localPosition.y
            );
        }

        private void LateUpdate()
        {
            if (Lebro.IsPaused || IsCanceled) return;
            UpdateToolTrait();
            UpdateCancellation();
        }

        private void UpdateCancellation()
        {
            if (cancelSprite == null) return;

            if (!IsCancellationEnabled || !Lebro.IsHovered)
            {
                cancelSprite.SetActive(false);
                return;
            }
            cancelSprite.SetActive(true);

            int token = GetPlayerCancelToken();
            if (token >= 0)
            {
                OnCancelledByPlayer?.Invoke();
                CancelTrait(token);
            }
        }

        protected virtual void CancelTrait(int cancelToken)
        {
            IsCanceled = true;
            Lebro.Personality.RemoveTrait(this);
        }

        protected abstract void UpdateToolTrait();

        protected abstract SpriteRendererConfig GetIconConfig();

        public abstract void LoadSettingsState(int index);
        public abstract int GetNextSettingsStateIndex(int currentIndex);

        protected abstract int GetPlayerCancelToken();
        public abstract bool CanBeGivenTo(Lebro lebro);
        public abstract bool CanBePlacedAt(Map map, Vector2Int cellPosition);
    }
}
