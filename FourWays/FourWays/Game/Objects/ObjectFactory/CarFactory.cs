using SFML.Graphics;
using System;
using System.Collections.Generic;

namespace FourWays.Game.Objects.ObjectFactory
{
    public class CarFactory
    {
        private const uint DEFAULT_WINDOW_WIDTH = 1280;
        private const uint DEFAULT_WINDOW_HEIGHT = 960;

        Func<Car, bool> CollideTest { get; }

        internal Texture CarTextureRight { get; private set; }
        internal Texture CarTextureLeft { get; private set; }
        internal Texture CarTextureUp { get; private set; }
        internal Texture CarTextureDown { get; private set; }

        public CarFactory(Func<Car, bool> collideTest)
        {
            CollideTest = collideTest;

            LoadContent();
        }

        internal void LoadContent()
        {
            CarTextureRight = new Texture(new Image("./Ressources/car_right.png"));
            CarTextureLeft = new Texture(new Image("./Ressources/car_left.png"));
            CarTextureUp = new Texture(new Image("./Ressources/car_up.png"));
            CarTextureDown = new Texture(new Image("./Ressources/car_down.png"));
        }

        internal void CarInit(Dictionary<Direction, List<Car>> cars, Dictionary<Direction, RoadLight> roadLights)
        {
            List<Car> temp;

            if (cars.TryGetValue(Direction.down, out temp)) temp.Add(PopACar(roadLights, Direction.down));

            if (cars.TryGetValue(Direction.up, out temp)) temp.Add(PopACar(roadLights, Direction.up));

            if (cars.TryGetValue(Direction.right, out temp)) temp.Add(PopACar(roadLights, Direction.right));

            if (cars.TryGetValue(Direction.left, out temp)) temp.Add(PopACar(roadLights, Direction.left));
        }

        internal Car CarCreation(Dictionary<Direction, RoadLight> roadLights)
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
            while (CollideTest(car));

            return PopACar(roadLights, direction);
        }

        private Car PopACar(Dictionary<Direction, RoadLight> roadLights, Direction direction)
        {
            roadLights.TryGetValue(direction, out RoadLight temp);

            return new Car(direction, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, CarTextureLeft);
        }
    }
}
