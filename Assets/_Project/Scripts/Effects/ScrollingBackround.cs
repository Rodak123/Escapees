using UnityEngine;

namespace GameJam
{
    public class ScrollingBackground : MonoBehaviour
    {
        [SerializeField] SpriteRenderer background;

        [Header("Animation")]
        [SerializeField, Min(0.001f)] private float scrollDuration = 60;

        private SpriteRenderer secondBackground;

        private Vector2 start;
        private Vector2 end;

        private float AnimationSpeed => 1f / Mathf.Max(scrollDuration, 0.001f);

        private float t;

        protected void Awake()
        {
            float width = background.sprite.bounds.size.x;

            start = background.transform.position;
            end = start - new Vector2(width, 0);

            secondBackground = Instantiate(background, background.transform);
            secondBackground.transform.position = start + new Vector2(width, 0);
        }

        private void Update()
        {
            t += AnimationSpeed * Time.deltaTime;
            if (t >= 1) t -= 1;

            background.transform.position = Vector2.Lerp(start, end, t);
        }
    }
}
