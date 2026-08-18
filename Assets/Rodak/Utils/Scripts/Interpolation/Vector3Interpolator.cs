using UnityEngine;

namespace Rodak.Animation.Interpolation
{
    public class Vector3Interpolator : IInterpolator<Vector3>
    {
        public Vector3 GetValue(float t, Vector3 start, Vector3 end)
        {
            return Vector3.LerpUnclamped(start, end, t);
        }
    }
}