using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameJam
{
    public class IntroScene : MonoBehaviour
    {
        [Serializable]
        public struct IntroCard
        {
            public Sprite Sprite;
            public float StartsAt;
        }

        [SerializeField] private IntroCard[] introCards;
        [SerializeField] private float autoExitTime;

        [Header("UI")]
        [SerializeField] private Image cardImage;

        private float introTimer;
        private bool introEnded;

        private int CurrentCardIndex
        {
            get
            {
                for (int i = introCards.Length - 1; i >= 0; i--)
                {
                    if (introCards[i].StartsAt <= introTimer) return i;
                }
                return -1;
            }
        }

        private void Start()
        {
            introTimer = 0;
        }

        private void Update()
        {
            if (introEnded) return;

            if (InputManager.Instance.WasGameNextLevelPressedThisFrame())
            {
                EndIntro();
                return;
            }

            UpdateUI();
            introTimer += Time.deltaTime;

            if (introTimer >= autoExitTime)
            {
                EndIntro();
            }
        }

        private void UpdateUI()
        {
            int index = CurrentCardIndex;
            if (index == -1) return;
            cardImage.sprite = introCards[index].Sprite;
        }

        public void EndIntro()
        {
            introEnded = true;
            SceneManager.LoadScene((int)GameScene.MainMenuScene);
        }

    }
}
