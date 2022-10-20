using FourWays.Game.Objects.CarFactory.CarComponents;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace FourWays.Game.Objects
{
    public class Car : GameObject
    {
        private Font Arial;

        private uint WindowWidth;
        private uint WindowHeight;

        private const float SecurityDistance = 15f;
        private const float MaxSpeed = 6f;
        private const float CarFrontSize = 40f;
        private const float CarSideSize = 60f;

        Func<Car, List<Car>> CollideTest { get; }
        internal Texture Texture { get; }

        internal Guid Guid;
        internal RectangleShape Shape { get; private set; }
        internal RoadLight RoadLight { get; }

        private Direction actualDirection;
        internal Direction direction
        {
            get => actualDirection;
            private set
            {
                actualDirection = value;
                AssignMove(value);
            }
        }

        internal CarState ActualStatus;
        internal CarState status
        {
            get => ActualStatus;
            set => ActualStatus = value;
        }

        private Vector2f move;

        internal Engine Engine;
        private Driver Driver;

        public Car(Direction direction, uint WindowWidth, uint WindowHeight, RoadLight roadLight, Func<Car, List<Car>> collideTest, Texture texture, Font arial)
        {
            Guid = Guid.NewGuid();
            Arial = arial;

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

            Engine = new Engine(Engine.Speed.Five, arial);
            Driver = new Driver(this);
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

        internal void LookAtRoadLights()
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

        internal List<Car> LookAtCars()
        {
            float multiplier = (float)Engine.RotationSpeed * 2;

            float distanceX = (move.X < 0 ? Shape.Size.X * multiplier : Shape.Size.X) * move.X;
            float distanceY = (move.Y < 0 ? Shape.Size.Y * multiplier : Shape.Size.Y) * move.Y;

            float recenterX = multiplier / 2 * Math.Abs(move.Y);
            float recenterY = multiplier / 2 * Math.Abs(move.X);

            RectangleShape shape = new RectangleShape(new Vector2f(Shape.Size.X * multiplier, Shape.Size.Y * multiplier));

            shape.Position = new Vector2f(Shape.Position.X + distanceX - recenterX, Shape.Position.Y + (distanceY) - recenterY);

            Car car = new Car(direction, WindowWidth, WindowHeight, RoadLight, CollideTest, Texture, Arial);
            car.Guid = Guid;
            car.Shape = shape;

            return CollideTest.Invoke(car);
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
            if (direction == Direction.right)
            {
                direction = direction;
            }
            Driver.Update();
        }

        private void Move()
        {
            Shape.Position = new Vector2f((float)(Shape.Position.X + move.X * Engine.RotationSpeed), (float)(Shape.Position.Y + move.Y * Engine.RotationSpeed));
        }

        internal void MoveForward(double moveStrength)
        {
            SpeedUp(moveStrength);
            Move();
        }

        private void MoveBack()
        {
            // do something

            Move();
        }
        private void SpeedUp(double moveStrength)
        {
            Engine.SpeedUp(moveStrength);
        }

        internal void SlowDown(double moveStrength)
        {
            Engine.Slowdown(moveStrength);
            Move();
        }

        private void Turn(Vector2f NewDirection)
        {
            // do something

            Move();
        }

        internal void UpgradeCore()
        {
            Engine.UpgradeCore();
        }
        internal void DowngradeCore()
        {
            Engine.DowngradeCore();
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