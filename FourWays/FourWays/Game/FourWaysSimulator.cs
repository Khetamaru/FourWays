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

        private const uint CAR_NUMBER_LIMIT = 6;
        private const string WINDOW_TITLE = "Four Ways";

        public uint DEATH_COUNTER = 0;

        private List<Car> rightRoadCars;
        private List<Car> leftRoadCars;
        private List<Car> upRoadCars;
        private List<Car> downRoadCars;

        private List<RectangleShape> roadBounds;
        private Dictionary<Direction, RoadLight> roadLights;

        private Texture OutRoadTexture;
        private Texture RoadCenterTexture;
        private Texture RoadHorizontalTexture;
        private Texture RoadVerticalTexture;
        public Texture CarTextureRight { get; private set; }
        public Texture CarTextureLeft { get; private set; }
        public Texture CarTextureUp { get; private set; }
        public Texture CarTextureDown { get; private set; }

        public FourWaysSimulator() : base(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, WINDOW_TITLE, Color.White) { }

        public override void LoadContent()
        {
            DebugUtility.LoadContent();
            LoadCarTextures();
            RoadBoundLoadTextures();
        }

        private void LoadCarTextures()
        {
            CarTextureRight = new Texture(new Image("./Ressources/car_right.png"));
            CarTextureLeft = new Texture(new Image("./Ressources/car_left.png"));
            CarTextureUp = new Texture(new Image("./Ressources/car_up.png"));
            CarTextureDown = new Texture(new Image("./Ressources/car_down.png"));
        }

        private void RoadBoundLoadTextures()
        {
            OutRoadTexture = new Texture(new Image("./Ressources/green-grass-texture_1249-15.jpg"));
            RoadCenterTexture = new Texture(new Image("./Ressources/road_center.jpg"));
            RoadHorizontalTexture = new Texture(new Image("./Ressources/road_horizontal.jpg"));
            RoadVerticalTexture = new Texture(new Image("./Ressources/road_vertical.jpg"));
        }

        public override void Initialize()
        {
            CreateLayer();
            CreateRoadLight();
            CreateCars();
        }

        private void CreateLayer()
        {
            roadBounds = new List<RectangleShape>();

            RectangleShape rectangleShape = new RectangleShape(new Vector2f(590f, 430f));
            rectangleShape.Position = new Vector2f(0f, 0f);
            rectangleShape.Texture = OutRoadTexture;
            roadBounds.Add(rectangleShape);

            rectangleShape = new RectangleShape(new Vector2f(590f, 430f));
            rectangleShape.Position = new Vector2f(0f, 530f);
            rectangleShape.Texture = OutRoadTexture;
            roadBounds.Add(rectangleShape);

            rectangleShape = new RectangleShape(new Vector2f(590f, 430f));
            rectangleShape.Position = new Vector2f(690f, 0f);
            rectangleShape.Texture = OutRoadTexture;
            roadBounds.Add(rectangleShape);

            rectangleShape = new RectangleShape(new Vector2f(590f, 430f));
            rectangleShape.Position = new Vector2f(690f, 530f);
            rectangleShape.Texture = OutRoadTexture;
            roadBounds.Add(rectangleShape);

            rectangleShape = new RectangleShape(new Vector2f(100f, 100f));
            rectangleShape.Position = new Vector2f(590f, 430f);
            rectangleShape.Texture = RoadCenterTexture;
            roadBounds.Add(rectangleShape);

            rectangleShape = new RectangleShape(new Vector2f(100f, 430f));
            rectangleShape.Position = new Vector2f(590f, 0f);
            rectangleShape.Texture = RoadVerticalTexture;
            roadBounds.Add(rectangleShape);

            rectangleShape = new RectangleShape(new Vector2f(100f, 430f));
            rectangleShape.Position = new Vector2f(590f, 530f);
            rectangleShape.Texture = RoadVerticalTexture;
            roadBounds.Add(rectangleShape);

            rectangleShape = new RectangleShape(new Vector2f(590f, 100f));
            rectangleShape.Position = new Vector2f(0f, 430f);
            rectangleShape.Texture = RoadHorizontalTexture;
            roadBounds.Add(rectangleShape);

            rectangleShape = new RectangleShape(new Vector2f(590f, 100f));
            rectangleShape.Position = new Vector2f(690f, 430f);
            rectangleShape.Texture = RoadHorizontalTexture;
            roadBounds.Add(rectangleShape);
        }

        private void CreateCars()
        {
            rightRoadCars = new List<Car>();
            leftRoadCars = new List<Car>();
            upRoadCars = new List<Car>();
            downRoadCars = new List<Car>();

            roadLights.TryGetValue(Direction.down, out RoadLight temp);
            downRoadCars.Add(new Car(Direction.down, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, CarTextureDown));

            roadLights.TryGetValue(Direction.up, out temp);
            upRoadCars.Add(new Car(Direction.up, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, CarTextureUp));

            roadLights.TryGetValue(Direction.right, out temp);
            rightRoadCars.Add(new Car(Direction.right, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, CarTextureRight));

            roadLights.TryGetValue(Direction.left, out temp);
            leftRoadCars.Add(new Car(Direction.left, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, CarTextureLeft));
        }

        private void CreateRoadLight()
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

        public override void Update(GameTime gameTime)
        {
            CarGeneration();
            foreach (KeyValuePair<Direction, RoadLight> roadLight in roadLights)
            {
                roadLight.Value.Update();
            }
            foreach (Car car in GetAllCars())
            {
                car.Update();
            }
            GarbageCycle();
        }

        private void CarGeneration()
        {
            if (GetAllCars().Count < CAR_NUMBER_LIMIT && ASpawnIsEmpty())
            {
                Car car;
                Direction direction;
                RoadLight temp;

                do
                {
                    direction = (Direction)Enum.GetValues(typeof(Direction)).GetValue(new Random().Next(4));
                    roadLights.TryGetValue(Direction.left, out temp);
                    car = new Car(direction, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, CarTextureLeft);
                }
                while (CollisionTest(car));

                switch (car.direction)
                {
                    case Direction.left:

                        roadLights.TryGetValue(Direction.left, out temp);
                        car = new Car(direction, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, CarTextureLeft);
                        leftRoadCars.Add(car);
                        break;

                    case Direction.right:

                        roadLights.TryGetValue(Direction.right, out temp);
                        car = new Car(direction, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, CarTextureRight);
                        rightRoadCars.Add(car);
                        break;

                    case Direction.up:

                        roadLights.TryGetValue(Direction.up, out temp);
                        car = new Car(direction, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, CarTextureUp);
                        upRoadCars.Add(car);
                        break;

                    case Direction.down:

                        roadLights.TryGetValue(Direction.down, out temp);
                        car = new Car(direction, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, CarTextureDown);
                        downRoadCars.Add(car);
                        break;
                }
            }
        }

        private bool ASpawnIsEmpty()
        {
            List<Car> cars = new List<Car>();
            roadLights.TryGetValue(Direction.left, out RoadLight temp);
            cars.Add(new Car(Direction.down, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, CarTextureLeft));
            cars.Add(new Car(Direction.up, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, CarTextureLeft));
            cars.Add(new Car(Direction.left, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, CarTextureLeft));
            cars.Add(new Car(Direction.right, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, CarTextureLeft));

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
                                DEATH_COUNTER++;
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
                    case Direction.left:

                        leftRoadCars.Remove(car);
                        break;

                    case Direction.right:

                        rightRoadCars.Remove(car);
                        break;

                    case Direction.up:

                        upRoadCars.Remove(car);
                        break;

                    case Direction.down:

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
            foreach (KeyValuePair<Direction, RoadLight> roadLight in roadLights)
            {
                Window.Draw(roadLight.Value.Image);
                //Window.Draw(roadLight.Value.StopArea);
            }
        }

        private List<Car> GetAllCars()
        {
            return rightRoadCars.Concat(leftRoadCars).Concat(upRoadCars).Concat(downRoadCars).ToList();
        }

        private bool CollideTest(Car carTest)
        {
            foreach(Car car in GetAllCars())
            {
                if (car.isColliding(carTest))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
