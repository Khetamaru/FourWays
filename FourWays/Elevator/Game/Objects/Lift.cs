using FourWays.Game.Objects;
using FourWays.Loop;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Elevator.Game.Objects
{
    public enum Direction
    {
        Up,
        Down,
        None
    }

    internal class Lift : GameObject
    {
        internal Direction direction;

        internal Person[] Space;

        internal int Floor;
        internal int Objectif;

        internal int X;
        internal int Y;

        internal Shape shape;

        internal Font ConsoleFont;

        internal int YFloorScale;

        public Lift(int x, int y, Font consoleFont, int yFloorScale)
        {
            X = x;
            Y = y;

            direction = Direction.None;
            Space = new Person[4];
            Floor = 0;
            Objectif = -1;
            ConsoleFont = consoleFont;
            YFloorScale = yFloorScale;

            ShapeCreation();
        }

        internal bool isObjectifNull() => Objectif >= 0;

        internal void TakeDirection()
        {
            if (isObjectifNull())
            {
                if(Objectif == Floor)
                {
                    direction = Direction.None;
                }
                else if(Objectif > Floor)
                {
                    direction = Direction.Up;
                }
                else if (Objectif < Floor)
                {
                    direction = Direction.Down;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        internal int SpaceUsed()
        {
            int i = 0;

            foreach (Person p in Space) { if (p != null) { i++; } }

            return i;
        }

        private void ShapeCreation()
        {
            shape = new RectangleShape(new Vector2f(40f, 60f));
            shape.Position = new Vector2f(X, Y);
            shape.FillColor = Color.White;
        }

        public override void Update()
        {

            TakeDirection();
            Move();
        }

        private void Move()
        {
            switch(direction)
            {
                case Direction.Up:
                    Y -= 1;
                    break; 

                case Direction.Down:
                    Y += 1;
                    break;

                default: break;
            }
            shape.Position = new Vector2f(shape.Position.X, Y);
        }

        public void Draw(GameLoop gameLoop)
        {
            gameLoop.Window.Draw(shape);

            Text text = new Text(SpaceUsed().ToString(), ConsoleFont, 14);
            text.Position = new Vector2f(X + 20, Y + 30);
            text.FillColor = Color.Red;
            gameLoop.Window.Draw(text);
        }
    }
}
