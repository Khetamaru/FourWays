using SFML.Graphics;
using SFML.System;
using System;

namespace FourWays.Game.Objects
{
    public class Car : GameObject
    {
        private uint WindowWidth;
        private uint WindowHeight;

        private const float SecurityDistance = 5f;
        private const float MaxSpeed = 6f;
        private const float CarFrontSize = 40f;
        private const float CarSideSize = 60f;

        Func<Car, bool> CollideTest { get; }
        public Texture Texture { get; }

        public Guid Guid;
        public RectangleShape Shape { get; private set; }
        private RoadLight RoadLight { get; }
        private Color Color { get; set; }
        private Direction actualDirection { get; set; }
        private float ActualSpeed { get; set; }
        private double AccuracyPourcentage { get; set; }
        public Direction direction
        {
            get
            {
                return actualDirection;
            }
            private set
            {
                actualDirection = value;
                AssignMove(value);
            }
        }
        public Status ActualStatus { get; set; }
        public Status status 
        {
            get
            {
                return ActualStatus;
            }
            set
            {
                ActualStatus = value;
                if (value == Status.Stop)
                {
                    Stop();
                }
            }
        }

        private Vector2f move { get; set; }

        public enum Direction
        {
            left,
            right,
            up,
            down
        }

        public enum Status
        {
            Stop,
            Go
        }

        public Car(Direction direction, uint WindowWidth, uint WindowHeight, RoadLight roadLight, Func<Car, bool> collideTest, Texture texture)
        {
            Guid = Guid.NewGuid();

            this.direction = direction;
            status = Status.Go;
            this.WindowWidth = WindowWidth;
            this.WindowHeight = WindowHeight;
            RoadLight = roadLight;
            CollideTest = collideTest;
            Texture = texture;

            ActualSpeed = 0f;
            AccuracyPourcentage = 0.0;

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
                    Shape.Position = new Vector2f(WindowWidth, (WindowHeight / 2) - 45);

                    break;

                case Direction.right:
                    Shape = new RectangleShape(new Vector2f(CarSideSize, CarFrontSize));
                    Shape.Position = new Vector2f(0f, (WindowHeight / 2) + 5f);

                    break;
            }
            Shape.Texture = Texture;
        }

        private void AssignMove(Direction direction)
        {
            switch (direction)
            {
                case Direction.left:

                    move = new Vector2f(-1, 0);
                    break;

                case Direction.right:

                    move = new Vector2f(1, 0);
                    break;

                case Direction.up:

                    move = new Vector2f(0, -1);
                    break;

                case Direction.down:

                    move = new Vector2f(0, 1);
                    break;
            }
        }

        private void Move()
        {
            AccuracyEvolution();

            Shape.Position = new Vector2f(Shape.Position.X + (move.X * ActualSpeed), Shape.Position.Y + (move.Y * ActualSpeed));
        }

        private void AccuracyEvolution()
        {
            AccuracyPourcentageUpdate();
            ActualSpeedUpdate();
        }

        private void AccuracyPourcentageUpdate()
        {
            if (1 - AccuracyPourcentage < 0.02)
            {
                AccuracyPourcentage = 1;
            }
            else
            {
                AccuracyPourcentage += 0.02;
            }
        }

        private void ActualSpeedUpdate()
        {
            ActualSpeed = (float)(MaxSpeed * AccuracyPourcentage);
        }

        private void Stop()
        {
            ActualSpeed = 0f;
            AccuracyPourcentage = 0.0;
        }

        public override void Update()
        {
            LookAtRoadLight();
            LookAtCarsInFront();

            if (status == Status.Go)
            {
                Move();
            }
            else if (isInTheStopArea() && RoadLight.state == RoadLight.State.Red && !isThereSomeOneInFront())
            {
                MoveToRoadLightLigne();
            }
        }

        private void LookAtRoadLight()
        {
            if (isInTheStopArea() && RoadLight.state == RoadLight.State.Red)
            {
                status = Status.Stop;
            }
            else
            {
                status = Status.Go;
            }
        }

        private void LookAtCarsInFront()
        {
            if (isThereSomeOneInFront())
            {
                status = Status.Stop;
            }
        }

        private void MoveToRoadLightLigne()
        {
            float x = Math.Abs(MaxSpeed * move.X);
            float x2 = Math.Abs(Shape.Position.X - RoadLight.StopArea.Position.X);
            float y = Math.Abs(MaxSpeed * move.Y);
            float y2 = Math.Abs(Shape.Position.Y - RoadLight.StopArea.Position.Y);

            switch (direction)
            {
                case Direction.left:

                    if (x < x2)
                    {
                        Move();
                    }
                    else
                    {
                        Shape.Position = new Vector2f(Shape.Position.X - x2, Shape.Position.Y);
                    }
                    break;

                case Direction.right:

                    if (x < x2)
                    {
                        Move();
                    }
                    else
                    {
                        Shape.Position = new Vector2f(Shape.Position.X + x2, Shape.Position.Y);
                    }
                    break;

                case Direction.up:

                    if (y < y2)
                    {
                        Move();
                    }
                    else
                    {
                        Shape.Position = new Vector2f(Shape.Position.X, Shape.Position.Y - y2);
                    }
                    break;

                case Direction.down:

                    if (y < y2)
                    {
                        Move();
                    }
                    else
                    {
                        Shape.Position = new Vector2f(Shape.Position.X, Shape.Position.Y + y2);
                    }
                    break;
            }
        }

        internal bool isColliding(Car car)
        {
            if (car.Guid != Guid)
            {
                return car.Shape.GetGlobalBounds().Intersects(Shape.GetGlobalBounds());
            }
            return false;
        }

        internal bool isInTheStopArea()
        {
            return Shape.GetGlobalBounds().Intersects(RoadLight.StopArea.GetGlobalBounds()) && isBehindTheRoadLight();
        }

        private bool isBehindTheRoadLight()
        {
            switch (direction)
            {
                case Direction.left: return Shape.Position.X >= RoadLight.StopArea.Position.X;
                case Direction.right: return Shape.Position.X <= RoadLight.StopArea.Position.X;
                case Direction.up: return Shape.Position.Y >= RoadLight.StopArea.Position.Y;
                case Direction.down: return Shape.Position.Y <= RoadLight.StopArea.Position.Y;
            }
            return true;
        }

        internal bool isThereSomeOneInFront()
        {
            RectangleShape shape = new RectangleShape(new Vector2f(Shape.Size.X, Shape.Size.Y));
            shape.Position = new Vector2f(Shape.Position.X + (SecurityDistance * MaxSpeed * move.X), Shape.Position.Y + (SecurityDistance * MaxSpeed * move.Y));

            Car car = new Car(direction, WindowWidth, WindowHeight, RoadLight, CollideTest, Texture);
            car.Guid = Guid;
            car.Shape = shape;

            return CollideTest.Invoke(car);
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