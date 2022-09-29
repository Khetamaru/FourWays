using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourWays.Game.Objects
{
    public class Car : GameObject
    {
        private uint WindowWidth;
        private uint WindowHeight;

        private const float Speed = 4f;
        private const float CarFrontSize = 40f;
        private const float CarSideSize = 60f;

        public Guid Guid;
        public RectangleShape Shape { get; private set; }
        private Color Color { get; set; }
        private Direction direction { get; set; }

        public enum Direction
        {
            left,
            right,
            up,
            down
        }

        public Car(Direction direction, uint WindowWidth, uint WindowHeight)
        {
            Guid = Guid.NewGuid();

            this.direction = direction;
            this.WindowWidth = WindowWidth;
            this.WindowHeight = WindowHeight;

            Color = Color.Red;
            switch (direction)
            {
                case Direction.down:
                    Shape = new RectangleShape(new Vector2f(CarFrontSize, CarSideSize));
                    Shape.Position = new Vector2f((WindowWidth / 2) - 45f, 0f);
                    break;

                case Direction.up:
                    Shape = new RectangleShape(new Vector2f(CarFrontSize, CarSideSize));
                    Shape.Position = new Vector2f((WindowWidth / 2) + 5f, WindowHeight);

                    break;

                case Direction.left:
                    Shape = new RectangleShape(new Vector2f(CarSideSize, CarFrontSize));
                    Shape.Position = new Vector2f(WindowWidth, (WindowHeight/ 2) - 45);

                    break;

                case Direction.right:
                    Shape = new RectangleShape(new Vector2f(CarSideSize, CarFrontSize));
                    Shape.Position = new Vector2f(0f, (WindowHeight / 2) + 5f);

                    break;
            }
            Shape.FillColor = Color;
        }

        public override void Update()
        {
            switch (direction)
            {
                case Direction.left:
                    Shape.Position = new Vector2f(Shape.Position.X - Speed, Shape.Position.Y);
                    break;

                case Direction.right:
                    Shape.Position = new Vector2f(Shape.Position.X + Speed, Shape.Position.Y);
                    break;

                case Direction.up:
                    Shape.Position = new Vector2f(Shape.Position.X, Shape.Position.Y - Speed);
                    break;

                case Direction.down:
                    Shape.Position = new Vector2f(Shape.Position.X, Shape.Position.Y + Speed);
                    break;
            }
        }

        internal bool isColliding(Car car)
        {
            return Shape.GetGlobalBounds().Intersects(car.Shape.GetGlobalBounds());
        }

        internal bool isOutOfBounds()
        {
            switch (direction)
            {
                case Direction.down: return Shape.Position.Y > WindowHeight;

                case Direction.up: return Shape.Position.Y + Shape.Size.Y < 0f;

                case Direction.right: return Shape.Position.X > WindowWidth;

                case Direction.left: return Shape.Position.X + Shape.Size.X < 0f;
            }
            return false;
        }
    }
}
