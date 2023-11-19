using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife.Game.Objects
{
    internal class Cell
    {
        private int X;
        private int Y;
        private bool Alive;

        private Shape deadShape;
        private Shape aliveShape;

        public Cell(int _X, int _Y, bool _StartAlive)
        {
            X = _X;
            Y = _Y;
            Alive = _StartAlive;


            Vector2f vector = new Vector2f(10, 10);

            deadShape = new RectangleShape(vector);
            deadShape.FillColor = Color.Black;
            deadShape.Position = new Vector2f(X * 10, Y * 10);

            aliveShape = new RectangleShape(vector);
            aliveShape.FillColor = Color.White;
            aliveShape.Position = new Vector2f(X * 10, Y * 10);
        }

        internal Shape Draw()
        {
            return Alive ? aliveShape : deadShape;
        }

        internal Cell Update(List<Cell> nabourCells)
        {
            if(Alive)
            {
                if (CountAliveCells(nabourCells) < 2 
                 || CountAliveCells(nabourCells) > 3) return new Cell(X, Y, false);
            }
            else
            {
                if (CountAliveCells(nabourCells) == 3) return new Cell(X, Y, true);
            }
            return this;
        }

        private int CountAliveCells(List<Cell> nabourCells)
        {
            int count = 0;

            foreach (Cell cell in nabourCells)
            {
                if (cell.Alive)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
