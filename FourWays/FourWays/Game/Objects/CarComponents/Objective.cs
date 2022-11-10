using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourWays.Game.Objects.CarComponents
{
    public class Objective
    {
        private const uint DEFAULT_WINDOW_WIDTH = 1280;
        private const uint DEFAULT_WINDOW_HEIGHT = 960;

        private const uint GROUND_WIDTH = (DEFAULT_WINDOW_WIDTH - 110) / 2;
        private const uint GROUND_HEIGHT = (DEFAULT_WINDOW_HEIGHT - 100) / 2;

        private const uint MARGIN = 0;

        internal Direction Direction;
        internal RectangleShape TurningZone;

        public Objective(Direction direction, uint WindowWidth, uint WindowHeight)
        {
            Random random = new Random();
            int i;

            do {
                i = random.Next(4);
                Direction = (Direction)i;
            }
            while (!IsObjectiveReachable(direction));

            switch (Direction)
            {
                case Direction.left:
                    TurningZone = new RectangleShape(new Vector2f(100f, 40f))
                    {
                        Position = new Vector2f(GROUND_WIDTH, WindowHeight / 2 - 42)
                    };
                    break;

                case Direction.right:
                    TurningZone = new RectangleShape(new Vector2f(100f, 40f))
                    {
                        Position = new Vector2f(GROUND_WIDTH, WindowHeight / 2 + 2f)
                    };
                    break;

                case Direction.up:
                    TurningZone = new RectangleShape(new Vector2f(40f, 100f))
                    {
                        Position = new Vector2f(WindowWidth / 2 - 3f, GROUND_HEIGHT)
                    };
                    break;

                case Direction.down:
                    TurningZone = new RectangleShape(new Vector2f(40f, 100f))
                    {
                        Position = new Vector2f(WindowWidth / 2 - 45f, GROUND_HEIGHT)
                    };
                    break;
            }
        }

        internal void ConvertShape(Car car)
        {
            RectangleShape rectangleShape = new RectangleShape();
            rectangleShape.Position = TurningZone.Position;
            rectangleShape.Position = Direction switch
            {
                Direction.left =>  new Vector2f((car.originalDirection == Direction.up ? TurningZone.Size.X : (TurningZone.Size.X / 2)) + TurningZone.Position.X - car.Shape.Size.Y + MARGIN, 
                                                TurningZone.Position.Y + MARGIN),
                Direction.right => new Vector2f((car.originalDirection == Direction.up ? (TurningZone.Size.X / 2) : 0) + TurningZone.Position.X + MARGIN,
                                                TurningZone.Position.Y + MARGIN),
                Direction.up =>    new Vector2f(TurningZone.Position.X + MARGIN,
                                               (car.originalDirection == Direction.right ? TurningZone.Size.Y : (TurningZone.Size.Y / 2)) + TurningZone.Position.Y - car.Shape.Size.X + MARGIN),
                Direction.down =>  new Vector2f(TurningZone.Position.X + MARGIN,
                                               (car.originalDirection == Direction.right ? (TurningZone.Size.Y / 2) : 0) + TurningZone.Position.Y + MARGIN),
                _ => throw new NotImplementedException()
            };
            rectangleShape.Size = new Vector2f(car.Shape.Size.Y, car.Shape.Size.X);
            car.Shape = rectangleShape;
        }

        internal bool IsObjectiveReachable(Direction direction)
        {
            return Direction switch
            {
                Direction.left => direction != Direction.right,
                Direction.right => direction != Direction.left,
                Direction.up => direction != Direction.down,
                Direction.down => direction != Direction.up,
                _ => throw new NotImplementedException()
            };
        }
        private bool IsTurningLeft(Direction direction)
        {
            return direction switch
            {
                Direction.down => Direction == Direction.right,
                Direction.right => Direction == Direction.up,
                Direction.up => Direction == Direction.left,
                Direction.left => Direction == Direction.down,
                _ => throw new NotImplementedException()
            };
        }
        internal bool IsInTurningZone(RectangleShape shape)
        {
            if (TurningZone.GetGlobalBounds().Intersects(shape.GetGlobalBounds(), out FloatRect overlap))
            {
                double zoneOne = overlap.Height * overlap.Width;
                double zoneTwo = shape.Size.X * shape.Size.Y;

                return (zoneOne / zoneTwo) * 100 >= 35;
            }
            else
            {
                return false;
            }
        }
        internal bool IsObjectiveReached(Direction direction) => Direction == direction;
        internal bool IsObjectiveCarable(Direction direction) => !IsObjectiveReached(direction) && IsObjectiveReachable(direction);
        internal bool IsCuttingWay(Direction direction) => IsObjectiveCarable(direction) && IsTurningLeft(direction);
    }
}
