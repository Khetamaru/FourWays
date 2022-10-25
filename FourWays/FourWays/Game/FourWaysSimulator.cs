using FourWays.Game.Objects;
using FourWays.Game.Objects.ObjectFactory;
using FourWays.Loop;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace FourWays.Game
{
    public class FourWaysSimulator : GameLoop
    {
        private Font Arial;

        private const uint DEFAULT_WINDOW_WIDTH = 1280;
        private const uint DEFAULT_WINDOW_HEIGHT = 960;

        private const string WINDOW_TITLE = "Four Ways";

        private const uint CAR_NUMBER_LIMIT = 8;
        internal uint DEATH_COUNTER = 0;

        private const bool TEST_ON = false;

        private List<RectangleShape> roadBounds;

        private Dictionary<Direction, List<Car>> cars;
        private Dictionary<Direction, RoadLight> roadLights;

        private CarFactory CarFactory;
        private RoadBoundFactory RoadBoundFactory;
        private RoadLightFactory RoadLightFactory;

        public FourWaysSimulator() : base(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, WINDOW_TITLE, Color.White)
        {
            Arial = new Font("./fonts/arial.ttf");

            CarFactory = new CarFactory(CollideTest, Arial, ExternalDraw, TEST_ON);
            RoadBoundFactory = new RoadBoundFactory();
            RoadLightFactory = new RoadLightFactory(ExternalDraw, TEST_ON);
        }

        internal override void LoadContent()
        {
            DebugUtility.LoadContent();

            CarFactory.LoadContent();
            RoadBoundFactory.LoadContent();
        }

        internal override void Initialize()
        {
            roadBounds = RoadBoundFactory.RoadBoundInit();
            roadLights = RoadLightFactory.RoadLightInit();
            cars = CarFactory.CarInit(roadLights);
        }

        internal override void Update(GameTime gameTime)
        {
            if (carsCount() < CAR_NUMBER_LIMIT && ASpawnIsEmpty())
            {
                var car = CarFactory.CarCreation(roadLights);
                if (cars.TryGetValue(car.direction, out List<Car> temp)) temp.Add(car);
            }

            foreach (KeyValuePair<Direction, RoadLight> roadLight in roadLights)
            {
                roadLight.Value.Update();
            }
            foreach (KeyValuePair<Direction, List<Car>> carList in cars)
            {
                foreach (Car car in carList.Value)
                {
                    car.Update();
                }
            }
            GarbageCycle();
        }

        private uint carsCount()
        {
            uint i = 0;
            foreach (KeyValuePair<Direction, List<Car>> carList in cars)
            {
                foreach (Car car in carList.Value)
                {
                    i++;
                }
            }
            return i;
        }

        private bool ASpawnIsEmpty()
        {
            List<Car> TestList = new List<Car>();

            roadLights.TryGetValue(Direction.left, out RoadLight temp);

            TestList.Add(new Car(Direction.down, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, ExternalDraw, null, Arial, TEST_ON));
            TestList.Add(new Car(Direction.up, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, ExternalDraw, null, Arial, TEST_ON));
            TestList.Add(new Car(Direction.left, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, ExternalDraw, null, Arial, TEST_ON));
            TestList.Add(new Car(Direction.right, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, ExternalDraw, null, Arial, TEST_ON));

            foreach (Car car in TestList)
            {
                if (!CollisionTest(car))
                {
                    return true;
                }
            }
            return false;
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

            foreach (KeyValuePair<Direction, List<Car>> carList in cars)
            {
                foreach (Car car in carList.Value)
                {
                    foreach (KeyValuePair<Direction, List<Car>> carList2 in cars)
                    {
                        foreach (Car car2 in carList2.Value)
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
                                        DEATH_COUNTER++;
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
            }
            return trashList;
        }

        private bool CollisionTest(Car car)
        {
            foreach (KeyValuePair<Direction, List<Car>> carList in cars)
            {
                foreach (Car car2 in carList.Value)
                {
                    if (car.isColliding(car2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void OutOfBoundsTest(List<Car> trashList)
        {
            foreach (KeyValuePair<Direction, List<Car>> carList in cars)
            {
                foreach (Car car in carList.Value) 
                {
                    if (car.isOutOfBounds())
                    {
                        try { trashList.Add(car); }
                        catch { }
                    }
                }
            }
        }

        private void EraseCollidedObjects(List<Car> trashList)
        {
            List<Car> temp;

            foreach (Car car in trashList)
            {
                switch (car.direction)
                {
                    case Direction.left:

                        if (cars.TryGetValue(Direction.left, out temp)) temp.Remove(car);
                        break;

                    case Direction.right:

                        if (cars.TryGetValue(Direction.right, out temp)) temp.Remove(car);
                        break;

                    case Direction.up:

                        if (cars.TryGetValue(Direction.up, out temp)) temp.Remove(car);
                        break;

                    case Direction.down:

                        if (cars.TryGetValue(Direction.down, out temp)) temp.Remove(car);
                        break;
                }
            }
        }

        internal override void Draw(GameTime gameTime)
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
            foreach (KeyValuePair<Direction, List<Car>> carList in cars)
            {
                foreach (Car car in carList.Value)
                {
                    Window.Draw(car.Shape);

                    car.Engine.speedText.Position = new Vector2f(car.Shape.Position.X + 10f, car.Shape.Position.Y + 10f);
                    Window.Draw(car.Engine.speedText);
                }
            }
        }

        private void DrawRoadLights()
        {
            foreach (KeyValuePair<Direction, RoadLight> roadLight in roadLights)
            {
                Window.Draw(roadLight.Value.Image);
                Window.Draw(roadLight.Value.StopLine);
            }
        }

        internal void ExternalDraw(RectangleShape rectangleShape)
        {
            Window.Draw(rectangleShape);

            Window.Display();
        }

        private List<Car> CollideTest(Car carTest)
        {
            List<Car> carsSeen = new List<Car>();

            foreach (KeyValuePair<Direction, List<Car>> carList in cars)
            {
                foreach (Car car in carList.Value)
                {
                    if (car.isColliding(carTest))
                    {
                        carsSeen.Add(car);
                    }
                }
            }
            return carsSeen;
        }
    }
}
