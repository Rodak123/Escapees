using UnityEngine;

namespace Rodak.Animation.Interpolation
{
    public class QuaternionInterpolator : IInterpolator<Quaternion>
    {
        public Quaternion GetValue(float t, Quaternion start, Quaternion end)
        {
            return Quaternion.SlerpUnclamped(start, end, t);
        }
    }
}