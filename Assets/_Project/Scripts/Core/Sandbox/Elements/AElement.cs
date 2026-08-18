using UnityEngine;

namespace GameJam
{
    public abstract class AElement
    {
        private bool hasTicked;

        public readonly Color Color;
        public bool HasTicked => hasTicked;

        public AElement(Color color)
        {
            Color = color;
        }

        public TickResult Tick(Simulation simulation, Vector2Int position)
        {
            if (hasTicked) return new()
            {
                Position = position,
                Element = this,
            };
            hasTicked = true;
            return EvaluateTick(simulation, position);
        }

        public void ClearTick()
        {
            hasTicked = false;
        }

        protected abstract TickResult EvaluateTick(Simulation simulation, Vector2Int position);
    }
}
