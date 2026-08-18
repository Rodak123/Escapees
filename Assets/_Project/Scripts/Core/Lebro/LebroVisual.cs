using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    [RequireComponent(typeof(Lebro))]
    public class LebroVisual : MonoBehaviour
    {
        [Serializable]
        public enum SpriteType
        {
            RunningLeft,
            RunningRight,
            Falling,
            FallingLethal,
            Standing,
            Death,
        }

        [SerializeField] private Transform visualContainer;

        [Header("Sprites")]
        [SerializeField] private GameObject standingSprite;
        [SerializeField] private GameObject fallingSprite;
        [SerializeField] private GameObject fallingLethalSprite;
        [SerializeField] private GameObject runningLeftSprite;
        [SerializeField] private GameObject runningRightSprite;
        [SerializeField] private GameObject deathSprite;

        [Header("Custom")]
        [SerializeField] private Transform customVisualContainer;

        private readonly Dictionary<SpriteType, GameObject> spriteVariants = new();
        private readonly Dictionary<SpriteType, GameObject> customSpriteVariants = new();

        private Lebro lebro;

        public SpriteType MoveSprite
        {
            get
            {
                if (lebro.IsDead)
                    return SpriteType.Death;

                if (!lebro.Controller.IsGrounded)
                    return lebro.Controller.IsFallLethal ? SpriteType.FallingLethal : SpriteType.Falling;

                if (Math.Abs(lebro.Controller.Velocity.x) >= 1)
                    return Math.Sign(lebro.Controller.Velocity.x) == 1 ? SpriteType.RunningRight : SpriteType.RunningLeft;

                return SpriteType.Standing;
            }
        }

        private void Awake()
        {
            lebro = GetComponent<Lebro>();

            spriteVariants.Add(SpriteType.RunningLeft, runningLeftSprite);
            spriteVariants.Add(SpriteType.RunningRight, runningRightSprite);
            spriteVariants.Add(SpriteType.Falling, fallingSprite);
            spriteVariants.Add(SpriteType.FallingLethal, fallingLethalSprite);
            spriteVariants.Add(SpriteType.Standing, standingSprite);
            spriteVariants.Add(SpriteType.Death, deathSprite);

            foreach (SpriteType spriteType in spriteVariants.Keys)
            {
                spriteVariants[spriteType].SetActive(false);
            }

            lebro.OnDeath += Lebro_OnDeath;
        }

        private void Update()
        {
            foreach (SpriteType spriteType in spriteVariants.Keys)
            {
                bool isCurrentMoveSprite = MoveSprite == spriteType;
                bool hasCustomSprite = customSpriteVariants.TryGetValue(spriteType, out GameObject customSprite);

                if (!isCurrentMoveSprite)
                {
                    if (hasCustomSprite) customSprite.SetActive(false);
                    spriteVariants[spriteType].SetActive(false);
                    continue;
                }

                if (hasCustomSprite) customSprite.SetActive(true);
                else spriteVariants[spriteType].SetActive(true);
            }
        }

        private void Lebro_OnDeath(Lebro lebro)
        {
            // spawn new temporary object that plays the animation
            GameObject deathAnimation = new("Lebro Death Animation");
            deathAnimation.transform.position = deathSprite.transform.position;

            deathAnimation.AddComponent<DestroyAfter>().Duration = 5;

            bool hasCustomSprite = customSpriteVariants.TryGetValue(SpriteType.Death, out GameObject customSprite);

            GameObject sprite = Instantiate(hasCustomSprite ? customSprite : deathSprite, deathAnimation.transform);
            sprite.SetActive(true);
        }

        public void EnableCustomVisualFor(SpriteType spriteType, GameObject visualPrefab)
        {
            GameObject newCustomSprite = Instantiate(visualPrefab, customVisualContainer);
            newCustomSprite.SetActive(false);

            if (customSpriteVariants.TryGetValue(spriteType, out GameObject customSprite))
            {
                customSprite.SetActive(false);
                Destroy(customSprite);
                customSpriteVariants[spriteType] = newCustomSprite;
            }
            else
            {
                customSpriteVariants.Add(spriteType, newCustomSprite);
            }

        }

        public void DisableCustomVisualFor(SpriteType spriteType)
        {
            if (customSpriteVariants.TryGetValue(spriteType, out GameObject customSprite))
            {
                Destroy(customSprite);
                customSpriteVariants.Remove(spriteType);
            }
        }
    }
}