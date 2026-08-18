using UnityEngine;

namespace Rodak.Animation.Interpolation
{
    // https://youtu.be/Lw8LPXPyrl0?si=3HeI09OX64yT1sdk
    public class Interpolated<T>
    {
        private T start;
        private T end;

        private float startTime = 0;
        private float speed = 1;

        private IInterpolator<T> interpolator;

        public T Start => start;
        public T End => end;

        public float Speed
        {
            get => speed;
            set => speed = value;
        }

        public float Duration
        {
            get => Speed == 0 ? 0 : 1 / Speed;
            set => Speed = value == 0 ? 0 : 1 / value;
        }

        public float StartTime => startTime;
        public float EndTime => startTime + Duration;

        public Interpolated(T initialValue, IInterpolator<T> interpolator)
        {
            start = initialValue;
            end = start;
            this.interpolator = interpolator;
        }

        public float GetElapsedTime()
        {
            return Time.time - startTime;
        }

        public void SetValue(T newValue)
        {
            start = newValue;
            end = newValue;
            startTime = Time.time;
        }

        public void SetTargetValue(T newValue)
        {
            start = GetValue();
            end = newValue;
            startTime = Time.time;
        }

        public T GetValue()
        {
            float elapsed = GetElapsedTime();
            float t = elapsed * speed;

            if (t >= 1f)
                return end;

            return interpolator.GetValue(t, start, end);
        }
    }
}