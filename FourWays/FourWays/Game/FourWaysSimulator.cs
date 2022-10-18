﻿using FourWays.Game.Objects;
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
        private RoadBoundFactory RoadBoundFactory;
        private RoadLightFactory RoadLightFactory;

        public FourWaysSimulator() : base(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, WINDOW_TITLE, Color.White)
        {
            CarFactory = new CarFactory(CollideTest);
            RoadBoundFactory = new RoadBoundFactory();
            RoadLightFactory = new RoadLightFactory();
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
            List<Car> TestList = new List<Car>();

            roadLights.TryGetValue(Direction.left, out RoadLight temp);

            TestList.Add(new Car(Direction.down, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, null));
            TestList.Add(new Car(Direction.up, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, null));
            TestList.Add(new Car(Direction.left, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, null));
            TestList.Add(new Car(Direction.right, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, null));

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
