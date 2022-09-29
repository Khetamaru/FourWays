using SFML.Graphics;
using SFML.System;
using FourWays.Loop;
using System.Collections.Generic;
using FourWays.Game.Objects;
using System;

namespace FourWays.Game
{
    public class FourWaysSimulator : GameLoop
    {
        public const uint DEFAULT_WINDOW_WIDTH = 1280;
        public const uint DEFAULT_WINDOW_HEIGHT = 960;

        public const uint CAR_NUMBER_LIMIT = 4;
        public const string WINDOW_TITLE = "Four Ways";

        public List<Car> cars;
        public List<RectangleShape> roadBounds;

        public FourWaysSimulator() : base(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, WINDOW_TITLE, Color.White) { }

        public override void LoadContent()
        {
            DebugUtility.LoadContent();
        }

        public override void Initialize()
        {
            CreateRoadBounds();
            CreateCars();
        }

        private void CreateCars()
        {
            cars = new List<Car>();

            cars.Add(new Car(Car.Direction.down, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT));
            cars.Add(new Car(Car.Direction.up, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT));
            cars.Add(new Car(Car.Direction.right, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT));
            cars.Add(new Car(Car.Direction.left, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT));
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

        public override void Update(GameTime gameTime)
        {
            CarGeneration();
            foreach (Car car in cars)
            {
                car.Update();
            }
            GarbageCycle();
        }

        private void CarGeneration()
        {
            Random rnd = new Random();

            if (cars.Count < CAR_NUMBER_LIMIT)
            {
                cars.Add(new Car((Car.Direction)Enum.GetValues(typeof(Car.Direction)).GetValue(rnd.Next(4)), DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT));
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

            foreach (Car car in cars)
            {
                foreach (Car car2 in cars)
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

        private void OutOfBoundsTest(List<Car> trashList)
        {
            foreach (Car car in cars)
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
                cars.Remove(car);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            DrawBackGround();
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
            foreach (Car car in cars)
            {
                Window.Draw(car.Shape);
            }
        }
    }
}
