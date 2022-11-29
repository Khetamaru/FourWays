using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FourWays.Game.Objects.Graphs.DeathGraph;

namespace FourWays.Game.Objects.CarChildren
{
    internal class RedCar : Car
    {
        internal static new DeathColor Color;
        internal static int Ponderation = 3;
        public RedCar(Direction direction, uint WindowWidth, uint WindowHeight, RoadLight roadLight, Func<Car, List<Car>> collideTest, Dictionary<Direction, Texture> texture, Font arial, DeathColor Color)
            : base(direction, WindowWidth, WindowHeight, roadLight, collideTest, texture, arial, Color)
        {
        }
    }
}
