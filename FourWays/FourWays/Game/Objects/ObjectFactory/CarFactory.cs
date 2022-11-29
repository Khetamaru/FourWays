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

        internal Dictionary<DeathColor, Dictionary<Direction, Texture>> Textures;

        internal Dictionary<DeathColor, int> ColorPondaration;

        public CarFactory(Func<Car, List<Car>> collideTest, Func<Car, List<Car>> collideTestSecurity, Font arial)
        {
            Arial = arial;
            CollideTest = collideTest;
            CollideTestSecurity = collideTestSecurity;
            Textures = new Dictionary<DeathColor, Dictionary<Direction, Texture>>();

            ColorPondarationInit();
        }

        private void ColorPondarationInit()
        {
            ColorPondaration = new Dictionary<DeathColor, int>();

            ColorPondaration.Add(DeathColor.red, RedCar.Ponderation);
            ColorPondaration.Add(DeathColor.blue, BlueCar.Ponderation);
            ColorPondaration.Add(DeathColor.green, GreenCar.Ponderation);
            ColorPondaration.Add(DeathColor.white, WhiteCar.Ponderation);
            ColorPondaration.Add(DeathColor.purple, PurpleCar.Ponderation);
            ColorPondaration.Add(DeathColor.pink, PinkCar.Ponderation);
            ColorPondaration.Add(DeathColor.yellow, YellowCar.Ponderation);
            ColorPondaration.Add(DeathColor.grey, GreyCar.Ponderation);
        }

        internal void LoadContent()
        {
            Dictionary<Direction, Texture> textures;
            foreach (string colorString in Enum.GetNames(typeof(DeathColor)))
            {
                textures = new Dictionary<Direction, Texture>();

                textures.Add(Direction.right, new Texture(new Image("./Ressources/" + colorString + "_right.png")));
                textures.Add(Direction.left, new Texture(new Image("./Ressources/" + colorString + "_left.png")));
                textures.Add(Direction.up, new Texture(new Image("./Ressources/" + colorString + "_up.png")));
                textures.Add(Direction.down, new Texture(new Image("./Ressources/" + colorString + "_down.png")));

                Textures.Add((DeathColor)Enum.Parse(typeof(DeathColor), colorString), textures);
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

            Texture texture = Textures.GetValueOrDefault(DeathColor.red).GetValueOrDefault(Direction.right);
            do
            {
                direction = (Direction)Enum.GetValues(typeof(Direction)).GetValue(new Random().Next(4));
                switch (direction)
                {
                    case Direction.left:
                        texture = Textures.GetValueOrDefault(DeathColor.red).GetValueOrDefault(Direction.left);
                        break;
                    case Direction.right:
                        texture = Textures.GetValueOrDefault(DeathColor.red).GetValueOrDefault(Direction.right);
                        break;
                    case Direction.up:
                        texture = Textures.GetValueOrDefault(DeathColor.red).GetValueOrDefault(Direction.up);
                        break;
                    case Direction.down:
                        texture = Textures.GetValueOrDefault(DeathColor.red).GetValueOrDefault(Direction.down);
                        break;
                }
                roadLights.TryGetValue(Direction.left, out temp);
                car = new Car(direction, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, temp, CollideTest, Textures.GetValueOrDefault(DeathColor.red), Arial, DeathColor.red);
            }
            while (CollideTestSecurity(car).Count > 0);

            return PopACar(roadLights, direction);
        }

        private Car PopACar(Dictionary<Direction, RoadLight> roadLights, Direction direction)
        {
            DeathColor deathColor = GetAColor();

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

        private DeathColor GetAColor()
        {
            int size = 0;

            foreach(KeyValuePair<DeathColor, int> keyValue in ColorPondaration)
            {
                size += keyValue.Value;
            }

            DeathColor[] deathColors = new DeathColor[size];
            int i = 0;
            int j = 0;

            foreach (KeyValuePair<DeathColor, int> keyValue in ColorPondaration)
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
    }
}
