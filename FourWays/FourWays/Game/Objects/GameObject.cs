using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourWays.Game.Objects
{
    public abstract class GameObject
    {
        protected GameObject()
        {

        }

        internal abstract void Update();
    }
}
