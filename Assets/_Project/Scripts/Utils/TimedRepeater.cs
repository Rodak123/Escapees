using System;

namespace GameJam
{
    public class TimedRepeater
    {
        public readonly float Interval;
        public readonly float Delay;

        private Action onTick;

        private float timer;

        public TimedRepeater(float interval, Action onTick)
        {
            Interval = interval;
            this.onTick = onTick;
        }

        public void Update(float deltaTime)
        {
            if (timer < Interval)
            {
                timer += deltaTime;
            }
            else
            {
                timer -= Interval;
                onTick?.Invoke();
            }
        }

        public void Reset()
        {
            timer = 0;
        }
    }
}
