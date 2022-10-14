using System;

namespace FourWays.Game.Objects
{
    public class Driver
    {
        Func<Car, bool> CollideTest { get; }

        public Driver(Func<Car, bool> collideTest)
        {
            CollideTest = collideTest;
        }
    }
}
