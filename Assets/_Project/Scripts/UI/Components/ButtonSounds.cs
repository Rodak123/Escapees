using UnityEngine;
using UnityEngine.UI;

namespace GameJam
{
    [RequireComponent(typeof(Button))]
    public class ButtonSounds : MonoBehaviour
    {
        private static int clickSoundIndex = 0;

        [SerializeField] private SoundEffect clickSound;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(Button_OnClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(Button_OnClick);
        }

        private void Button_OnClick()
        {
            if (clickSound.Clips.Length == 0) return;
            clickSoundIndex %= clickSound.Clips.Length;

            AudioClip currentClickSound = clickSound.Clips[clickSoundIndex];
            SFXManager.Instance.PlayClip(currentClickSound, clickSound.VolumeScale);

            clickSoundIndex++;
        }
    }
}
