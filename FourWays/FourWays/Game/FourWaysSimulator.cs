using FourWays.Game.Objects;
using FourWays.Game.Objects.Graphs;
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

        private const GAME_MODE TEST_ON = GAME_MODE.TEST_MODE_2;
        private enum GAME_MODE
        {
            DEFAULT,
            TEST_MODE_1,
            TEST_MODE_2
        }

        private bool RENDER_SPEED;
        private bool RENDER_OBJECTIVE;
        private bool RENDER_STOP_LINE;
        private bool RENDER_TURNING_ZONE;
        private bool RENDER_SHADE;

        internal DeathGraph DeathGraph;

        private List<RectangleShape> roadBounds;

        private Dictionary<Direction, List<Car>> cars;
        private Dictionary<Direction, RoadLight> roadLights;

        private CarFactory CarFactory;
        private RoadBoundFactory RoadBoundFactory;
        private RoadLightFactory RoadLightFactory;

        public FourWaysSimulator() : base(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, WINDOW_TITLE, Color.White)
        {
            Arial = new Font("./fonts/arial.ttf");

            DeathGraph = new DeathGraph(this, new Vector2f(DEFAULT_WINDOW_WIDTH, 15f), Color.Cyan);

            CarFactory = new CarFactory(CollideTest, CollideTestSecurity, Arial);
            RoadBoundFactory = new RoadBoundFactory();
            RoadLightFactory = new RoadLightFactory();

            TestMode();
        }

        private void TestMode()
        {
            switch(TEST_ON)
            {
                case GAME_MODE.DEFAULT:

                    RENDER_SPEED = false;
                    RENDER_OBJECTIVE = false;
                    RENDER_STOP_LINE = false;
                    RENDER_TURNING_ZONE = false;
                    RENDER_SHADE = false;
                    break;

                case GAME_MODE.TEST_MODE_1:

                    RENDER_SPEED = true;
                    RENDER_OBJECTIVE = false;
                    RENDER_STOP_LINE = true;
                    RENDER_TURNING_ZONE = false;
                    RENDER_SHADE = false;
                    break;

                case GAME_MODE.TEST_MODE_2:

                    RENDER_SPEED = false;
                    RENDER_OBJECTIVE = true;
                    RENDER_STOP_LINE = false;
                    RENDER_TURNING_ZONE = false;
                    RENDER_SHADE = true;
                    break;
            }
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

            TestList.Add(new Car(Direction.down, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, null, Arial));
            TestList.Add(new Car(Direction.up, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, null, Arial));
            TestList.Add(new Car(Direction.left, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, null, Arial));
            TestList.Add(new Car(Direction.right, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, null, Arial));

            foreach (Car car in TestList)
            {
                if (CollideTestSecurity(car).Count == 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void GarbageCycle()
        {
            List<Car> trashList = GetCollidedCars();
            OutOfBoundsTest(trashList);
            EraseCollidedObjects(trashList);
        }

        private List<Car> GetCollidedCars()
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
                                        Console.WriteLine((Math.Round(Time.FromSeconds(GameTime.TotalTimeElapsed).AsSeconds() / 60, 0, MidpointRounding.ToNegativeInfinity) +
                                         "m:" +
                                         Math.Round(Time.FromSeconds(GameTime.TotalTimeElapsed).AsSeconds() % 60, 0) +
                                         "s") + " Car " + car.Guid.ToString() + " collide !");

                                        trashList.Add(car);
                                        trashList.Add(car2);
                                        DeathGraph.AddDeathCounter(DeathGraph.DeathColor.Red);
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

        private List<Car> CollideTest(Car carTest)
        {
            List<Car> carsSeen = new List<Car>();

            foreach (KeyValuePair<Direction, List<Car>> carList in cars)
            {
                foreach (Car car in carList.Value)
                {
                    if (car.isColliding(carTest) && car.Guid != carTest.Guid)
                    {
                        carsSeen.Add(car);
                    }
                }
            }
            return carsSeen;
        }

        private List<Car> CollideTestSecurity(Car carTest)
        {
            List<Car> carsSeen = new List<Car>();

            foreach (KeyValuePair<Direction, List<Car>> carList in cars)
            {
                foreach (Car car in carList.Value)
                {
                    if (car.isColliding(carTest) || carTest.Driver.GetDistance(car.Shape) <= carTest.SecurityDistance)
                    {
                        carsSeen.Add(car);
                    }
                }
            }
            return carsSeen;
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
                switch (car.originalDirection)
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
            DeathGraph.DrawDataTab();
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
            bool trigger = false;
            foreach (KeyValuePair<Direction, List<Car>> carList in cars)
            {
                foreach (Car car in carList.Value)
                {
                    if (RENDER_TURNING_ZONE)
                    {
                        car.Objective.TurningZone.FillColor = Color.Black;
                        Window.Draw(car.Objective.TurningZone);
                    }
                    Window.Draw(car.Shape);
                    if (RENDER_SPEED)
                    {
                        car.Engine.speedText.Position = new Vector2f(car.Shape.Position.X + 10f, car.Shape.Position.Y + 10f);
                        Window.Draw(car.Engine.speedText);
                    }
                    if (RENDER_OBJECTIVE)
                    {
                        Text objectiveText = new Text(car.Objective.Direction.ToString().ToUpper(), Arial, 20);
                        objectiveText.OutlineThickness = 3;
                        objectiveText.FillColor = Color.Blue;
                        objectiveText.OutlineColor = Color.Cyan;
                        objectiveText.Position = new Vector2f(car.Shape.Position.X + 10f, car.Shape.Position.Y + 10f);

                        Window.Draw(objectiveText);
                    }
                    if (RENDER_SHADE && !trigger)
                    {
                        Window.Draw(car.ShowShade());
                        trigger = true;
                    }
                }
            }
        }

        private void DrawRoadLights()
        {
            foreach (KeyValuePair<Direction, RoadLight> roadLight in roadLights)
            {
                Window.Draw(roadLight.Value.Image);
                if (RENDER_STOP_LINE) Window.Draw(roadLight.Value.StopLine);
            }
        }
    }
}
