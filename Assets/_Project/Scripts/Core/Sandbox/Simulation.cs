using System;

namespace GameJam
{
    // inspired by https://www.youtube.com/watch?v=5Ka3tbbT-9E&list=WL&index=36
    public class Simulation
    {
        private readonly AElement[,] grid;

        public int Width => grid.GetLength(0);
        public int Height => grid.GetLength(1);

        public Simulation(int width, int height)
        {
            grid = new AElement[width, height];
        }

        public void Tick()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    AElement element = grid[x, y];
                    if (element == null)
                        continue;
                    element.ClearTick();
                }
            }

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    AElement element = grid[x, y];

                    if (element == null)
                        continue;

                    TickResult result = element.Tick(this, new(x, y));

                    grid[x, y] = null;
                    grid[result.Position.x, result.Position.y] = result.Element;
                }
            }
        }

        public bool IsPositionValid(int x, int y)
        {
            if (x < 0 || x >= Width) return false;
            if (y < 0 || y >= Height) return false;
            return true;
        }

        public AElement GetElementAt(int x, int y)
        {
            if (!IsPositionValid(x, y)) throw new IndexOutOfRangeException($"Position ({x}, {y}) is outside range.");
            return grid[x, y];
        }

        public bool TryGetElementAt(int x, int y, out AElement element)
        {
            if (!IsPositionValid(x, y))
            {
                element = null;
                return false;
            }

            element = grid[x, y];
            return true;
        }

        public bool TryGetElementAt<T>(int x, int y, out T element) where T : AElement
        {
            if (!IsPositionValid(x, y))
            {
                element = null;
                return false;
            }

            AElement anyElement = grid[x, y];

            if (anyElement is T foundElement)
            {
                element = foundElement;
                return true;
            }

            element = null;
            return false;
        }

        public bool TryRemoveElement(AElement targetElement)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    AElement element = grid[x, y];
                    if (element == targetElement)
                    {
                        grid[x, y] = null;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool TryRemoveElementAt(int x, int y)
        {
            return TrySetElementAt(x, y, null);
        }

        public bool TrySetElementAt(int x, int y, AElement element)
        {
            if (!IsPositionValid(x, y)) return false;

            grid[x, y] = element;
            return true;
        }
    }
}
