using FourWays.Game.Objects.CarChildren;
using FourWays.Game.Objects.CarComponents;
using FourWays.Game.Objects.CarFactory.CarComponents;
using FourWays.Game.Objects.Graphs;
using FourWays.Game.Objects.ObjectFactory;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using static FourWays.Game.Objects.Graphs.DeathGraph;

namespace FourWays.Game.Objects
{
    public class Car : GameObject
    {
        private Font Arial;

        private uint WindowWidth;
        private uint WindowHeight;

        private const float CarFrontSize = 40f;
        private const float CarSideSize = 60f;
        private Func<Car, List<Car>> CollideTest { get; }
        internal Dictionary<Direction, Texture> Texture { get; }

        internal Guid Guid;
        internal RectangleShape Shape;
        internal RoadLight RoadLight { get; }

        private Direction actualDirection;

        internal Direction originalDirection;
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

        internal Vector2f move;

        internal Engine Engine;
        internal Driver Driver;
        internal Objective Objective;
        internal int Limitor;

        internal CarColor Color;

        public Car(Direction direction, uint WindowWidth, uint WindowHeight, RoadLight roadLight, Func<Car, List<Car>> collideTest, Dictionary<Direction, Texture> texture, Font arial, CarColor Color)
        {
            Guid = Guid.NewGuid();
            Arial = arial;
            this.direction = direction;
            originalDirection = direction;
            status = CarState.Go;
            this.WindowWidth = WindowWidth;
            this.WindowHeight = WindowHeight;
            RoadLight = roadLight;
            CollideTest = collideTest;
            Texture = texture;

            switch (direction)
            {
                case Direction.down:
                    Shape = new RectangleShape(new Vector2f(CarFrontSize, CarSideSize));
                    Shape.Position = new Vector2f(WindowWidth / 2 - 45f, 0f);
                    break;

                case Direction.up:
                    Shape = new RectangleShape(new Vector2f(CarFrontSize, CarSideSize));
                    Shape.Position = new Vector2f(WindowWidth / 2 - 3f, WindowHeight - Shape.Size.Y);
                    break;

                case Direction.left:
                    Shape = new RectangleShape(new Vector2f(CarSideSize, CarFrontSize));
                    Shape.Position = new Vector2f(WindowWidth - Shape.Size.X, WindowHeight / 2 - 42);
                    break;

                case Direction.right:
                    Shape = new RectangleShape(new Vector2f(CarSideSize, CarFrontSize));
                    Shape.Position = new Vector2f(0f, WindowHeight / 2 + 2f);
                    break;
            }
            if (Texture != null)
            {
                Texture textureTemp = Texture.GetValueOrDefault(direction);
                Shape.Texture = textureTemp;
            }
            Engine = new Engine(Engine.Speed.Three, arial);
            Driver = new Driver(this);
            Objective = new Objective(this, WindowWidth, WindowHeight);
            Limitor = 3;
            this.Color = Color;
        }

        private void AssignMove(Direction direction)
        {
            move = direction switch
            {
                Direction.left => new Vector2f(-1, 0),
                Direction.right => new Vector2f(1, 0),
                Direction.up => new Vector2f(0, -1),
                Direction.down => new Vector2f(0, 1),
                _ => throw new NotImplementedException()
            };
        }

        internal float SecurityDistance => (int)Engine.BoxSpeed > 0 ? CarSideSize * (int)Engine.BoxSpeed : CarSideSize;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        internal override void Update()
        {
            Driver.Update();
        }

        private void Move()
        {
            Shape.Position = new Vector2f((float)(Shape.Position.X + move.X * Engine.RotationSpeed), (float)(Shape.Position.Y + move.Y * Engine.RotationSpeed));
        }

        internal void MoveForward()
        {
            if (!LimitorTrigger()) SpeedUp();
            Move();
        }

        internal void MoveBack(double moveStrength)
        {
            if (Engine.BoxSpeed != Engine.Speed.Back) Engine.TryPassBackSpeed();

            SlowDown(moveStrength);
        }

        private void SpeedUp()
        {
            Engine.SpeedUp();
        }

        internal void SlowDown(double moveStrength)
        {
            Engine.Slowdown(moveStrength);
            Move();
        }

        internal void Turn(Direction NewDirection)
        {
            Objective.ConvertShape(this);
            direction = Objective.Direction;
            ChangeTexture();
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

        private void ChangeTexture()
        {
            Texture textureTemp = Texture.GetValueOrDefault(direction);
            if (textureTemp != null) Shape.Texture = textureTemp;
        }

        private bool LimitorTrigger() => IsBehindTheLine() && (int)Engine.BoxSpeed >= Limitor;
        internal bool AbleToTurn() => Objective.IsInTurningZone(Shape) && !CollisionAfterTurning();

        private bool CollisionAfterTurning()
        {
            RectangleShape shape = new RectangleShape(new Vector2f(Shape.Size.X, Shape.Size.Y));

            shape.Position = new Vector2f(Shape.Position.X, Shape.Position.Y);

            Car car = new Car(originalDirection, WindowWidth, WindowHeight, RoadLight, CollideTest, Texture, Arial, CarColor.red);
            car.Guid = Guid;
            car.Shape = shape;
            car.Objective = Objective;
            car.Turn(Objective.Direction);

            Car NeerestCar = Driver.AnalyseCarsSeen(LookCars());

            return CollideTest.Invoke(car).Count > 0 || 
                  (NeerestCar != null && 
                   Driver.GetDistance(NeerestCar.Shape) <= NeerestCar.SecurityDistance);
        }

        private RectangleShape GenerateShade()
        {
            float multiplier = (float)Engine.RotationSpeed * 2;

            RectangleShape shape = new RectangleShape(new Vector2f(Math.Max(Shape.Size.X, Shape.Size.Y) * (multiplier < 1 ? 1 : multiplier), Math.Max(Shape.Size.X, Shape.Size.Y) * (multiplier < 1 ? 1 : multiplier)));

            float diffX = (shape.Size.X / 2) - (Shape.Size.X / 2);
            float diffY = (shape.Size.Y / 2) - (Shape.Size.Y / 2);

            switch (direction)
            {
                case Direction.left:
                    shape.Position = new Vector2f(Shape.Position.X + -shape.Size.X, Shape.Position.Y - diffY);
                    break;
                case Direction.right:
                    shape.Position = new Vector2f(Shape.Position.X + Shape.Size.X, Shape.Position.Y - diffY);
                    break;
                case Direction.up:
                    shape.Position = new Vector2f(Shape.Position.X - diffX, Shape.Position.Y + -shape.Size.Y);
                    break;
                case Direction.down:
                    shape.Position = new Vector2f(Shape.Position.X - diffX, Shape.Position.Y + Shape.Size.Y);
                    break;
            }

            return shape;
        }

        internal List<Car> LookCars()
        {
            RectangleShape shape = GenerateShade();

            Car car = new Car(originalDirection, WindowWidth, WindowHeight, RoadLight, CollideTest, Texture, Arial, CarColor.red);
            car.Guid = Guid;
            car.Shape = shape;

            return CollideTest.Invoke(car);
        }

        internal RectangleShape ShowShade()
        {
            RectangleShape shape = GenerateShade();

            shape.FillColor = SFML.Graphics.Color.Yellow;

            return shape;
        }

        internal bool isColliding(Car car)
        {
            if (car.Guid != Guid)
            {
                return car.Shape.GetGlobalBounds().Intersects(Shape.GetGlobalBounds());
            }
            return false;
        }

        internal bool IsCarAlign(RectangleShape shape)
        {
            RectangleShape Shape = new RectangleShape(this.Shape);
            Shape.Position = new Vector2f(this.Shape.Position.X + (this.Shape.Size.X * move.X), this.Shape.Position.Y + (this.Shape.Size.Y * move.Y));

            return Shape.GetGlobalBounds().Intersects(shape.GetGlobalBounds());
        }

        internal RoadLight LookAtRoadLights()
        {
            return (IsBehindTheLine() && (RoadLight.state == RoadLightState.Red || RoadLight.state == RoadLightState.Orange)) ? RoadLight : null;
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

        internal bool IsBehindTheLine()
        {
            return originalDirection switch
            {
                Direction.left => RoadLight.StopLine.Position.X < Shape.Position.X,
                Direction.right => RoadLight.StopLine.Position.X > Shape.Position.X + Shape.Size.X,
                Direction.up => RoadLight.StopLine.Position.Y < Shape.Position.Y,
                Direction.down => RoadLight.StopLine.Position.Y > Shape.Position.Y + Shape.Size.Y,
                _ => false
            };
        }
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
        Go,
        Decelerate,
        Turning
    }
}