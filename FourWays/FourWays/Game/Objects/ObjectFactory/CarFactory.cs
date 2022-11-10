using SFML.Graphics;
using System;
using System.Collections.Generic;

namespace FourWays.Game.Objects.ObjectFactory
{
    public class CarFactory
    {
        private Font Arial;

        private const uint DEFAULT_WINDOW_WIDTH = 1280;
        private const uint DEFAULT_WINDOW_HEIGHT = 960;
        
        Func<Car, List<Car>> CollideTest { get; }
        Func<Car, List<Car>> CollideTestSecurity { get; }

        internal Dictionary<Direction, Texture> Texture;

        public CarFactory(Func<Car, List<Car>> collideTest, Func<Car, List<Car>> collideTestSecurity, Font arial)
        {
            Arial = arial;
            CollideTest = collideTest;
            CollideTestSecurity = collideTestSecurity;
            Texture = new Dictionary<Direction, Texture>();
        }

        internal void LoadContent()
        {
            Texture.Add(Direction.right, new Texture(new Image("./Ressources/car_right.png")));
            Texture.Add(Direction.left, new Texture(new Image("./Ressources/car_left.png")));
            Texture.Add(Direction.up, new Texture(new Image("./Ressources/car_up.png")));
            Texture.Add(Direction.down, new Texture(new Image("./Ressources/car_down.png")));
        }

        internal Dictionary<Direction, List<Car>> CarInit(Dictionary<Direction, RoadLight> roadLights)
        {
            Dictionary<Direction, List<Car>> cars = new Dictionary<Direction, List<Car>>();

            cars.Add(Direction.left, new List<Car>());
            cars.Add(Direction.right, new List<Car>());
            cars.Add(Direction.up, new List<Car>());
            cars.Add(Direction.down, new List<Car>());

            List<Car> temp;

            if (cars.TryGetValue(Direction.down, out temp)) temp.Add(PopACar(roadLights, Direction.down));
            if (cars.TryGetValue(Direction.up, out temp)) temp.Add(PopACar(roadLights, Direction.up));
            if (cars.TryGetValue(Direction.right, out temp)) temp.Add(PopACar(roadLights, Direction.right));
            if (cars.TryGetValue(Direction.left, out temp)) temp.Add(PopACar(roadLights, Direction.left));

            return cars;
        }

        internal Car CarCreation(Dictionary<Direction, RoadLight> roadLights)
        {
            Car car;
            Direction direction;
            RoadLight temp;

            Texture texture = Texture.GetValueOrDefault(Direction.right);
            do
            {
                direction = (Direction)Enum.GetValues(typeof(Direction)).GetValue(new Random().Next(4));
                switch (direction)
                {
                    case Direction.left:
                        texture = Texture.GetValueOrDefault(Direction.left);
                        break;
                    case Direction.right:
                        texture = Texture.GetValueOrDefault(Direction.right);
                        break;
                    case Direction.up:
                        texture = Texture.GetValueOrDefault(Direction.up);
                        break;
                    case Direction.down:
                        texture = Texture.GetValueOrDefault(Direction.down);
                        break;
                }
                roadLights.TryGetValue(Direction.left, out temp);
                car = new Car(direction, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, Texture, Arial);
            }
            while (CollideTestSecurity(car).Count > 0);

            return PopACar(roadLights, direction);
        }

        private Car PopACar(Dictionary<Direction, RoadLight> roadLights, Direction direction)
        {
            roadLights.TryGetValue(direction, out RoadLight temp);
            Texture texture = Texture.GetValueOrDefault(Direction.right);
            switch (direction)
            {
                case Direction.left:
                    texture = Texture.GetValueOrDefault(Direction.left);
                    break;
                case Direction.right:
                    texture = Texture.GetValueOrDefault(Direction.right);
                    break;
                case Direction.up:
                    texture = Texture.GetValueOrDefault(Direction.up);
                    break;
                case Direction.down:
                    texture = Texture.GetValueOrDefault(Direction.down);
                    break;
            }
            return new Car(direction, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, Texture, Arial);
        }
    }
}
