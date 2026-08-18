using UnityEngine;

namespace GameJam
{
    [RequireComponent(typeof(MapSandbox))]
    public class MapSandboxVisual : MonoBehaviour
    {
        [SerializeField] private Color32 emptyColor;
        [SerializeField] private SpriteRenderer mapSprite;

        private MapSandbox mapSandbox;

        private Texture2D texture;
        private Color32[] pixels;

        private void Awake()
        {
            mapSandbox = GetComponent<MapSandbox>();
        }

        private void Start()
        {
            int width = mapSandbox.Simulation.Width;
            int height = mapSandbox.Simulation.Height;

            pixels = new Color32[width * height];
            texture = new(width, height)
            {
                filterMode = FilterMode.Point,
            };
        }

        private void LateUpdate()
        {
            int width = mapSandbox.Simulation.Width;
            int height = mapSandbox.Simulation.Height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int index = x + y * width;
                    AElement element = mapSandbox.Simulation.GetElementAt(x, y);

                    pixels[index] = element?.Color ?? emptyColor;
                }
            }

            texture.SetPixels32(0, 0, width, height, pixels);
            texture.Apply();

            mapSprite.sprite = Sprite.Create(texture, new(new(0, 0), new(width, height)), new(0, 0), 1);
        }
    }
}
