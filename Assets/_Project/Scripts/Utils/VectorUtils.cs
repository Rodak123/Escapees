using UnityEngine;

namespace GameJam
{
    public static class VectorUtils
    {
        public static Vector2Int RoundToInt(this Vector2 vector)
        {
            return new(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y)); ;
        }

        public static Vector2Int RoundToInt(this Vector3 vector)
        {
            return new(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y)); ;
        }
    }
}
