﻿using FourWays.Game.Objects.ObjectFactory;
using SFML.Graphics;
using System;
using System.Collections.Generic;

namespace FourWays.Game.Objects.CarChildren
{
    internal class GreenCar : Car
    {
        internal static new CarColor Color;
        internal static int Ponderation = 0;
        public GreenCar(Direction direction, uint WindowWidth, uint WindowHeight, RoadLight roadLight, Func<Car, List<Car>> collideTest, Dictionary<Direction, Texture> texture, Font arial, CarColor Color)
            : base(direction, WindowWidth, WindowHeight, roadLight, collideTest, texture, arial, Color)
        {
        }
    }
}