using UnityEngine;

namespace GameJam
{
    // Immovable colored material
    public class ColoredStone : ASolid
    {
        private bool markedToPulverize;

        public ColoredStone(Color color) : base(color)
        { }

        protected override TickResult EvaluateTick(Simulation simulation, Vector2Int position)
        {
            return new()
            {
                Position = position,
                Element = markedToPulverize ? new ColoredSand(Color) : this,
            };
        }

        public void Pulverize()
        {
            markedToPulverize = true;
        }
    }
}
