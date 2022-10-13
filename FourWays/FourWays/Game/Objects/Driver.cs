using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
