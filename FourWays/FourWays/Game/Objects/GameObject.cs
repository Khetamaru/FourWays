using SFML.Graphics;
using System;
using System.Collections.Generic;

namespace FourWays.Game.Objects
{
    public abstract class GameObject
    {
        private Action<RectangleShape> ExternalDrawFunction;
        protected GameObject()
        {

        }

        internal abstract void Update();

        internal abstract void BreakPointHighlight(bool switchTrigger);
    }
}
