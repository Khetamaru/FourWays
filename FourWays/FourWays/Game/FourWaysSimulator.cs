using SFML.Graphics;
using SFML.System;
using FourWays.Loop;
using System.Collections.Generic;
using FourWays.Game.Objects;
using System;
using System.Linq;

namespace FourWays.Game
{
    public class FourWaysSimulator : GameLoop
    {
        private const uint DEFAULT_WINDOW_WIDTH = 1280;
        private const uint DEFAULT_WINDOW_HEIGHT = 960;

        private const uint CAR_NUMBER_LIMIT = 4;
        private const string WINDOW_TITLE = "Four Ways";

        private List<Car> rightRoadCars;
        private List<Car> leftRoadCars;
        private List<Car> upRoadCars;
        private List<Car> downRoadCars;

        private List<RectangleShape> roadBounds;
        private List<RoadLight> roadLights;

        public FourWaysSimulator() : base(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, WINDOW_TITLE, Color.White) { }

        public override void LoadContent()
        {
            DebugUtility.LoadContent();
        }

        public override void Initialize()
        {
            CreateRoadBounds();
            CreateCars();
            CreateRoadLight();
        }

        private void CreateRoadBounds()
        {
            roadBounds = new List<RectangleShape>();

            RectangleShape rectangleShape = new RectangleShape(new Vector2f(590f, 430f));
            rectangleShape.Position = new Vector2f(0f, 0f);
            rectangleShape.FillColor = Color.Black;
            roadBounds.Add(rectangleShape);

            rectangleShape = new RectangleShape(new Vector2f(590f, 430f));
            rectangleShape.Position = new Vector2f(0f, 530f);
            rectangleShape.FillColor = Color.Black;
            roadBounds.Add(rectangleShape);

            rectangleShape = new RectangleShape(new Vector2f(590f, 430f));
            rectangleShape.Position = new Vector2f(690f, 0f);
            rectangleShape.FillColor = Color.Black;
            roadBounds.Add(rectangleShape);

            rectangleShape = new RectangleShape(new Vector2f(590f, 430f));
            rectangleShape.Position = new Vector2f(690f, 530f);
            rectangleShape.FillColor = Color.Black;
            roadBounds.Add(rectangleShape);
        }

        private void CreateCars()
        {
            rightRoadCars = new List<Car>();
            leftRoadCars = new List<Car>();
            upRoadCars = new List<Car>();
            downRoadCars = new List<Car>();

            downRoadCars.Add(new Car(Car.Direction.down, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT));
            upRoadCars.Add(new Car(Car.Direction.up, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT));
            rightRoadCars.Add(new Car(Car.Direction.right, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT));
            leftRoadCars.Add(new Car(Car.Direction.left, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT));
        }

        private void CreateRoadLight()
        {
            roadLights = new List<RoadLight>();
            RectangleShape rightStopArea = new RectangleShape(new Vector2f(60f, 40f));
            RectangleShape downStopArea = new RectangleShape(new Vector2f(40f, 60f));
            RectangleShape upStopArea = new RectangleShape(new Vector2f(40f, 60f));
            RectangleShape leftStopArea = new RectangleShape(new Vector2f(60f, 40f));

            rightStopArea.Position = new Vector2f(590f - 60f, 430f + 55f);
            downStopArea.Position = new Vector2f(590f + 5f, 430f - 60f);
            upStopArea.Position = new Vector2f(590f + 55f, 430f + +100f);
            leftStopArea.Position = new Vector2f(590f + 100f, 430f + 5f);

            roadLights.Add(new RoadLight(new Vector2f(580f, 535f), Car.Direction.right, rightStopArea));
            roadLights.Add(new RoadLight(new Vector2f(530f, 350f), Car.Direction.down, downStopArea));
            roadLights.Add(new RoadLight(new Vector2f(700f, 535f), Car.Direction.up, upStopArea));
            roadLights.Add(new RoadLight(new Vector2f(700f, 420f), Car.Direction.left, leftStopArea));
        }

        public override void Update(GameTime gameTime)
        {
            CarGeneration();
            foreach (RoadLight roadLight in roadLights)
            {
                roadLight.Update();
            }
            RoadLightCall();
            foreach (Car car in GetAllCars())
            {
                car.Update();
            }
            GarbageCycle();
        }

        private void CarGeneration()
        {
            if (GetAllCars().Count < CAR_NUMBER_LIMIT)
            {
                Car car;
                do
                {
                    Car.Direction direction = (Car.Direction)Enum.GetValues(typeof(Car.Direction)).GetValue(new Random().Next(4));
                    car = new Car(direction, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT);
                }
                while (CollisionTest(car));

                switch (car.direction)
                {
                    case Car.Direction.left:

                        leftRoadCars.Add(car);
                        break;

                    case Car.Direction.right:

                        rightRoadCars.Add(car);
                        break;

                    case Car.Direction.up:

                        upRoadCars.Add(car);
                        break;

                    case Car.Direction.down:

                        downRoadCars.Add(car);
                        break;
                }
            }
        }

        private void RoadLightCall()
        {
            foreach (RoadLight roadLight in roadLights)
            {
                if (roadLight.state == RoadLight.State.Red)
                {
                    switch (roadLight.direction)
                    {
                        case Car.Direction.left:

                            RedCall(leftRoadCars, roadLight.StopArea);
                            break;

                        case Car.Direction.right:

                            RedCall(rightRoadCars, roadLight.StopArea);
                            break;

                        case Car.Direction.up:

                            RedCall(upRoadCars, roadLight.StopArea);
                            break;

                        case Car.Direction.down:

                            RedCall(downRoadCars, roadLight.StopArea);
                            break;
                    }
                }
            }
        }

        private void RedCall(List<Car> roadCars, RectangleShape stopArea)
        {
            foreach (Car car in roadCars)
            {
                if (car.isInTheStopArea(stopArea))
                {
                    car.status = Car.Status.Stop;
                }
            }
        }

        private void GarbageCycle()
        {
            List<Car> trashList = CollisionTest();
            OutOfBoundsTest(trashList);

            EraseCollidedObjects(trashList);
        }

        private List<Car> CollisionTest()
        {
            List<Car> trashList = new List<Car>();

            foreach (Car car in GetAllCars())
            {
                foreach (Car car2 in GetAllCars())
                {
                    if (car != car2)
                    {
                        if (car.isColliding(car2))
                        {
                            try
                            {
                                Console.WriteLine("Car " + car.Guid.ToString() + " collide !");

                                trashList.Add(car);
                                trashList.Add(car2);
                            }
                            catch { }
                        }
                    }
                }
            }

            return trashList;
        }

        private bool CollisionTest(Car car)
        {
            foreach (Car car2 in GetAllCars())
            {
                if (car.isColliding(car2))
                {
                    return true;
                }
            }
            return false;
        }

        private void OutOfBoundsTest(List<Car> trashList)
        {
            foreach (Car car in GetAllCars())
            {
                if (car.isOutOfBounds())
                {
                    try
                    {
                        trashList.Add(car);
                    }
                    catch { }
                }
            }
        }

        private void EraseCollidedObjects(List<Car> trashList)
        {
            foreach (Car car in trashList)
            {
                switch (car.direction)
                {
                    case Car.Direction.left:

                        leftRoadCars.Remove(car);
                        break;

                    case Car.Direction.right:

                        rightRoadCars.Remove(car);
                        break;

                    case Car.Direction.up:

                        upRoadCars.Remove(car);
                        break;

                    case Car.Direction.down:

                        downRoadCars.Remove(car);
                        break;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            DrawBackGround();
            DrawRoadLights();
            DrawCars();

            DebugUtility.DrawPerformanceData(this, Color.White);
        }

        private void DrawBackGround()
        {
            foreach (RectangleShape rectangle in roadBounds)
            {
                Window.Draw(rectangle);
            }
        }

        private void DrawCars()
        {
            foreach (Car car in GetAllCars())
            {
                Window.Draw(car.Shape);
            }
        }

        private void DrawRoadLights()
        {
            foreach (RoadLight roadLight in roadLights)
            {
                Window.Draw(roadLight.Image);
                Window.Draw(roadLight.StopArea);
            }
        }

        private List<Car> GetAllCars()
        {
            return rightRoadCars.Concat(leftRoadCars).Concat(upRoadCars).Concat(downRoadCars).ToList();
        }
    }
}
