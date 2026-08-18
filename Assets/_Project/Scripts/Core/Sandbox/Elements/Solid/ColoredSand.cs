using UnityEngine;

namespace GameJam
{
    // Movable colored material
    public class ColoredSand : ASolid
    {
        public ColoredSand(Color color) : base(color)
        { }

        protected override TickResult EvaluateTick(Simulation simulation, Vector2Int position)
        {
            return new()
            {
                Position = GetNextPosition(simulation, position),
                Element = this,
            };
        }

        private Vector2Int GetNextPosition(Simulation simulation, Vector2Int position)
        {
            Vector2Int below = new(position.x, position.y - 1);
            if (simulation.TryGetElementAt(below.x, below.y, out AElement elementBelow) && elementBelow == null)
            {
                return below; // move down
            }

            int side = UnityEngine.Random.value > 0.5 ? 1 : -1;

            Vector2Int sideA = new(position.x + side, position.y - 1);
            if (simulation.TryGetElementAt(sideA.x, sideA.y, out AElement valueA) && valueA == null)
            {
                return sideA; // move to side A
            }

            Vector2Int sideB = new(position.x - side, position.y - 1);
            if (simulation.TryGetElementAt(sideB.x, sideB.y, out AElement valueB) && valueB == null)
            {
                return sideB; // move to side B
            }

            return position; // stay
        }
    }
}
