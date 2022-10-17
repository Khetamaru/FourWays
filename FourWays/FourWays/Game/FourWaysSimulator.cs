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
        private const uint DEFAULT_WINDOW_WIDTH = 1280;
        private const uint DEFAULT_WINDOW_HEIGHT = 960;

        private const string WINDOW_TITLE = "Four Ways";

        private const uint CAR_NUMBER_LIMIT = 6;
        internal uint DEATH_COUNTER = 0;

        private List<RectangleShape> roadBounds;

        private Dictionary<Direction, List<Car>> cars;
        private Dictionary<Direction, RoadLight> roadLights;

        private CarFactory CarFactory;
        private RoadBoundFactory RoadLightFactory;

        public FourWaysSimulator() : base(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, WINDOW_TITLE, Color.White)
        {
            CarFactory = new CarFactory(CollideTest);
            RoadLightFactory = new RoadBoundFactory();
        }

        internal override void LoadContent()
        {
            DebugUtility.LoadContent();

            CarFactory.LoadContent();
            RoadLightFactory.LoadContent();
        }

        internal override void Initialize()
        {
            RoadBoundInit();
            RoadLightInit();
            CarInit();
        }

        private void RoadBoundInit()
        {
            roadBounds = new List<RectangleShape>();

            RoadLightFactory.RoadBoundInit();
        }

        private void CarInit()
        {
            cars.Add(Direction.left, new List<Car>());
            cars.Add(Direction.right, new List<Car>());
            cars.Add(Direction.up, new List<Car>());
            cars.Add(Direction.down, new List<Car>());

            CarFactory.CarInit(cars, roadLights);
        }

        private void RoadLightInit()
        {
            roadLights = new Dictionary<Direction, RoadLight>();

            RectangleShape rightStopArea = new RectangleShape(new Vector2f(60f, 40f));
            RectangleShape downStopArea = new RectangleShape(new Vector2f(40f, 60f));
            RectangleShape upStopArea = new RectangleShape(new Vector2f(40f, 60f));
            RectangleShape leftStopArea = new RectangleShape(new Vector2f(60f, 40f));

            RectangleShape rightDecelerateArea = new RectangleShape(new Vector2f(60f, 40f));
            RectangleShape downDecelerateArea = new RectangleShape(new Vector2f(40f, 60f));
            RectangleShape upDecelerateArea = new RectangleShape(new Vector2f(40f, 60f));
            RectangleShape leftDecelerateArea = new RectangleShape(new Vector2f(60f, 40f));

            rightStopArea.Position = new Vector2f(590f - 60f, 430f + 55f);
            downStopArea.Position = new Vector2f(590f + 5f, 430f - 60f);
            upStopArea.Position = new Vector2f(590f + 55f, 430f + +100f);
            leftStopArea.Position = new Vector2f(590f + 100f, 430f + 5f);

            rightDecelerateArea.Position = new Vector2f(590f - 120f, 430f + 55f);
            downDecelerateArea.Position = new Vector2f(590f + 5f, 430f - 120f);
            upDecelerateArea.Position = new Vector2f(590f + 55f, 430f + +160f);
            leftDecelerateArea.Position = new Vector2f(590f + 160f, 430f + 5f);

            roadLights.Add(Direction.right, new RoadLight(new Vector2f(580f, 535f), Direction.right, rightStopArea, rightDecelerateArea, RoadLightState.Green));
            roadLights.Add(Direction.down, new RoadLight(new Vector2f(530f, 350f), Direction.down, downStopArea, downDecelerateArea, RoadLightState.Red));
            roadLights.Add(Direction.up, new RoadLight(new Vector2f(700f, 535f), Direction.up, upStopArea, upDecelerateArea, RoadLightState.Red));
            roadLights.Add(Direction.left, new RoadLight(new Vector2f(700f, 420f), Direction.left, leftStopArea, leftDecelerateArea, RoadLightState.Green));

            roadLights.TryGetValue(Direction.left, out RoadLight tempLeft);
            roadLights.TryGetValue(Direction.right, out RoadLight tempRight);
            roadLights.TryGetValue(Direction.up, out RoadLight tempUp);
            roadLights.TryGetValue(Direction.down, out RoadLight tempDown);

            tempLeft.AssignRoadLightLeft(tempUp);
            tempUp.AssignRoadLightLeft(tempRight);
            tempRight.AssignRoadLightLeft(tempDown);
            tempDown.AssignRoadLightLeft(tempLeft);
        }

        internal override void Update(GameTime gameTime)
        {
            if (cars.Count < CAR_NUMBER_LIMIT && ASpawnIsEmpty())
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

        private bool ASpawnIsEmpty()
        {
            List<Car> cars = new List<Car>();
            roadLights.TryGetValue(Direction.left, out RoadLight temp);
            cars.Add(new Car(Direction.down, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, new Texture(0, 0)));
            cars.Add(new Car(Direction.up, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, new Texture(0, 0)));
            cars.Add(new Car(Direction.left, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, new Texture(0, 0)));
            cars.Add(new Car(Direction.right, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, new Texture(0, 0)));

            foreach (Car car in cars)
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
                    foreach (Car car2 in carList.Value)
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
                }
            }
        }

        private void DrawRoadLights()
        {
            foreach (KeyValuePair<Direction, RoadLight> roadLight in roadLights)
            {
                Window.Draw(roadLight.Value.Image);
                //Window.Draw(roadLight.Value.StopArea);
            }
        }

        private bool CollideTest(Car carTest)
        {
            foreach (KeyValuePair<Direction, List<Car>> carList in cars)
            {
                foreach (Car car in carList.Value)
                {
                    if (car.isColliding(carTest))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
