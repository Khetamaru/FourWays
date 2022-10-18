using FourWays.Game.Objects.CarFactory.CarComponents;
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
        private const double AccuracyPourcentageStackValue = 0.002;

        Func<Car, bool> CollideTest { get; }
        internal Texture Texture { get; }

        internal Guid Guid;
        internal RectangleShape Shape { get; private set; }
        private RoadLight RoadLight { get; }

        private Direction actualDirection;
        internal Direction direction
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

        internal CarState ActualStatus;
        internal CarState status
        {
            get
            {
                return ActualStatus;
            }
            set
            {
                ActualStatus = value;
            }
        }

        private Vector2f move;

        private Engine Core;

        public Car(Direction direction, uint WindowWidth, uint WindowHeight, RoadLight roadLight, Func<Car, bool> collideTest, Texture texture)
        {
            Guid = Guid.NewGuid();

            this.direction = direction;
            status = CarState.Go;
            this.WindowWidth = WindowWidth;
            this.WindowHeight = WindowHeight;
            RoadLight = roadLight;
            CollideTest = collideTest;

            switch (direction)
            {
                case Direction.down:
                    Shape = new RectangleShape(new Vector2f(CarFrontSize, CarSideSize));
                    Shape.Position = new Vector2f(WindowWidth / 2 - 45f, 0f);
                    break;

                case Direction.up:
                    Shape = new RectangleShape(new Vector2f(CarFrontSize, CarSideSize));
                    Shape.Position = new Vector2f(WindowWidth / 2 + 5f, WindowHeight);

                    break;

                case Direction.left:
                    Shape = new RectangleShape(new Vector2f(CarSideSize, CarFrontSize));
                    Shape.Position = new Vector2f(WindowWidth, WindowHeight / 2 - 45);

                    break;

                case Direction.right:
                    Shape = new RectangleShape(new Vector2f(CarSideSize, CarFrontSize));
                    Shape.Position = new Vector2f(0f, WindowHeight / 2 + 5f);

                    break;
            }
            if (texture != null) Shape.Texture = texture;

            Core = new Engine(0f, Engine.Speed.One);
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

        private void GetInfos()
        {
            LookAtRoadLight();
            LookAtCarsInFront();
        }

        private void ChooseAnAction()
        {
            switch (status)
            {
                case CarState.Go:

                    Move();
                    break;

                case CarState.Decelerate:

                    SlowDown(0.001f);
                    break;

                default:

                    if (isInTheStopArea() && RoadLight.state == RoadLightState.Red && !isThereSomeOneInFront())
                    {
                        SlowDown(0.001f);
                        //MoveToRoadLightLigne();
                    }
                    break;
            }
        }

        private void LookAtRoadLight()
        {
            if (isInTheStopArea() && RoadLight.state == RoadLightState.Red)
            {
                status = CarState.Stop;
            }
            if (isInTheDecelerateArea() && RoadLight.state == RoadLightState.Red)
            {
                status = CarState.Decelerate;
            }
            else
            {
                status = CarState.Go;
            }
        }

        private void LookAtCarsInFront()
        {
            if (isThereSomeOneInFront())
            {
                status = CarState.Decelerate;
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

        internal bool isInTheDecelerateArea()
        {
            return Shape.GetGlobalBounds().Intersects(RoadLight.DecelerateArea.GetGlobalBounds()) && isBehindTheRoadLight();
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
            shape.Position = new Vector2f(Shape.Position.X + SecurityDistance * MaxSpeed * move.X, Shape.Position.Y + SecurityDistance * MaxSpeed * move.Y);

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

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        internal override void Update()
        {
            GetInfos();
            ChooseAnAction();
        }

        private void Move()
        {
            Shape.Position = new Vector2f((float)(Shape.Position.X + move.X * Core.RotationSpeed), (float)(Shape.Position.Y + move.Y * Core.RotationSpeed));
        }

        private void MoveForward()
        {
            // do something

            Move();
        }

        private void MoveBack()
        {
            // do something

            Move();
        }
        private void SpeedUp(float moveStrength)
        {
            Core.SpeedUp(moveStrength);
        }

        private void SlowDown(float moveStrength)
        {
            Core.Slowdown(moveStrength);
        }

        private void Turn(Vector2f NewDirection)
        {
            // do something

            Move();
        }

        private void UpgradeCore()
        {
            Core.UpgradeCore();
        }
        private void DowngradeCore()
        {
            Core.DowngradeCore();
        }

        private void LookForward() { }
        private void LookBack() { }
    }

    public enum Direction
    {
        left,
        right,
        up,
        down
    }

    public enum CarState
    {
        Stop,
        Go,
        Decelerate
    }
}