using FourWays.Game.Objects.CarChildren;
using FourWays.Game.Objects.Graphs;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using static FourWays.Game.Objects.Graphs.DeathGraph;

namespace FourWays.Game.Objects.ObjectFactory
{
    public class CarFactory
    {
        private Font Arial;

        private const uint DEFAULT_WINDOW_WIDTH = 1280;
        private const uint DEFAULT_WINDOW_HEIGHT = 960;

        internal Func<Car, List<Car>> CollideTest { get; }
        internal Func<Car, List<Car>> CollideTestSecurity { get; }

        internal Dictionary<CarColor, Dictionary<Direction, Texture>> Textures;

        internal Dictionary<CarColor, int> ColorPondaration;

        public CarFactory(Func<Car, List<Car>> collideTest, Func<Car, List<Car>> collideTestSecurity, Font arial)
        {
            Arial = arial;
            CollideTest = collideTest;
            CollideTestSecurity = collideTestSecurity;
            Textures = new Dictionary<CarColor, Dictionary<Direction, Texture>>();

            ColorPondarationInit();
        }

        private void ColorPondarationInit()
        {
            ColorPondaration = new Dictionary<CarColor, int>();

            ColorPondaration.Add(CarColor.red, RedCar.Ponderation);
            ColorPondaration.Add(CarColor.blue, BlueCar.Ponderation);
            ColorPondaration.Add(CarColor.green, GreenCar.Ponderation);
            ColorPondaration.Add(CarColor.white, WhiteCar.Ponderation);
            ColorPondaration.Add(CarColor.purple, PurpleCar.Ponderation);
            ColorPondaration.Add(CarColor.pink, PinkCar.Ponderation);
            ColorPondaration.Add(CarColor.yellow, YellowCar.Ponderation);
            ColorPondaration.Add(CarColor.grey, GreyCar.Ponderation);
        }

        internal void LoadContent()
        {
            Dictionary<Direction, Texture> textures;
            foreach (string colorString in Enum.GetNames(typeof(CarColor)))
            {
                textures = new Dictionary<Direction, Texture>();

                textures.Add(Direction.right, new Texture(new Image("./Ressources/" + colorString + "_right.png")));
                textures.Add(Direction.left, new Texture(new Image("./Ressources/" + colorString + "_left.png")));
                textures.Add(Direction.up, new Texture(new Image("./Ressources/" + colorString + "_up.png")));
                textures.Add(Direction.down, new Texture(new Image("./Ressources/" + colorString + "_down.png")));

                Textures.Add((CarColor)Enum.Parse(typeof(CarColor), colorString), textures);
            }
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

            Texture texture = Textures.GetValueOrDefault(CarColor.red).GetValueOrDefault(Direction.right);
            do
            {
                direction = (Direction)Enum.GetValues(typeof(Direction)).GetValue(new Random().Next(4));
                switch (direction)
                {
                    case Direction.left:
                        texture = Textures.GetValueOrDefault(CarColor.red).GetValueOrDefault(Direction.left);
                        break;
                    case Direction.right:
                        texture = Textures.GetValueOrDefault(CarColor.red).GetValueOrDefault(Direction.right);
                        break;
                    case Direction.up:
                        texture = Textures.GetValueOrDefault(CarColor.red).GetValueOrDefault(Direction.up);
                        break;
                    case Direction.down:
                        texture = Textures.GetValueOrDefault(CarColor.red).GetValueOrDefault(Direction.down);
                        break;
                }
                roadLights.TryGetValue(Direction.left, out temp);
                car = new Car(direction, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, Textures.GetValueOrDefault(CarColor.red), Arial, CarColor.red);
            }
            while (CollideTestSecurity(car).Count > 0);

            return PopACar(roadLights, direction);
        }

        private Car PopACar(Dictionary<Direction, RoadLight> roadLights, Direction direction)
        {
            CarColor deathColor = GetAColor();

            roadLights.TryGetValue(direction, out RoadLight temp);
            Texture texture = Textures.GetValueOrDefault(deathColor).GetValueOrDefault(Direction.right);
            switch (direction)
            {
                case Direction.left:
                    texture = Textures.GetValueOrDefault(deathColor).GetValueOrDefault(Direction.left);
                    break;
                case Direction.right:
                    texture = Textures.GetValueOrDefault(deathColor).GetValueOrDefault(Direction.right);
                    break;
                case Direction.up:
                    texture = Textures.GetValueOrDefault(deathColor).GetValueOrDefault(Direction.up);
                    break;
                case Direction.down:
                    texture = Textures.GetValueOrDefault(deathColor).GetValueOrDefault(Direction.down);
                    break;
            }
            return new Car(direction, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, Textures.GetValueOrDefault(deathColor), Arial, deathColor);
        }

        private CarColor GetAColor()
        {
            int size = 0;

            foreach(KeyValuePair<CarColor, int> keyValue in ColorPondaration)
            {
                size += keyValue.Value;
            }

            CarColor[] deathColors = new CarColor[size];
            int i = 0;
            int j = 0;

            foreach (KeyValuePair<CarColor, int> keyValue in ColorPondaration)
            {
                while(j < keyValue.Value)
                {
                    deathColors[i] = keyValue.Key;
                    i++;
                    j++;
                }
                j = 0;
            }
            return deathColors[new Random().Next(size)];
        }

        public List<CarColor> ActiveColors()
        {
            List<CarColor> colors = new List<CarColor>();

            if (RedCar.Ponderation > 0) colors.Add(CarColor.red);
            if (BlueCar.Ponderation > 0) colors.Add(CarColor.blue);
            if (GreenCar.Ponderation > 0) colors.Add(CarColor.green);
            if (WhiteCar.Ponderation > 0) colors.Add(CarColor.white);
            if (PurpleCar.Ponderation > 0) colors.Add(CarColor.purple);
            if (PinkCar.Ponderation > 0) colors.Add(CarColor.pink);
            if (GreyCar.Ponderation > 0) colors.Add(CarColor.grey);
            if (YellowCar.Ponderation > 0) colors.Add(CarColor.yellow);

            return colors;
        }
    }

    public enum CarColor
    {
        red,
        blue,
        green,
        grey,
        purple,
        yellow,
        pink,
        white
    }
}
