using System.Collections.Generic;

namespace SnakeMultiplayer.Models
{
    public class Snake
    {
        public List<Point> Body { get; set; } = new List<Point>();
        public Direction CurrentDirection { get; set; }
        public string Color { get; set; }

        public Snake(Point startPosition, Direction initialDirection, string color)
        {
            Body.Add(startPosition);
            CurrentDirection = initialDirection;
            Color = color;
        }

        public Point Head => Body[0];

        public void Move(int gridWidth, int gridHeight)
        {
            Point newHead = new Point(Head.X, Head.Y);
            switch (CurrentDirection)
            {
                case Direction.Up:
                    newHead.Y = (newHead.Y - 1 + gridHeight) % gridHeight;
                    break;
                case Direction.Down:
                    newHead.Y = (newHead.Y + 1) % gridHeight;
                    break;
                case Direction.Left:
                    newHead.X = (newHead.X - 1 + gridWidth) % gridWidth;
                    break;
                case Direction.Right:
                    newHead.X = (newHead.X + 1) % gridWidth;
                    break;
            }
            Body.Insert(0, newHead);
            Body.RemoveAt(Body.Count - 1);
        }

        public void Grow()
        {
            Body.Add(Body[Body.Count - 1]);
        }
    }
}
