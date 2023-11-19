using FourWays.Loop;
using System;
using System.Linq;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using GameOfLife.Game.Objects;
using System.Threading;

namespace GameOfLife.Game
{
    internal class GameOfLifeEngine : GameLoop
    {
        private const uint DEFAULT_WINDOW_WIDTH = 900;
        private const uint DEFAULT_WINDOW_HEIGHT = 900;
        private const string WINDOW_TITLE = "Game Of Life";

        private int TimeCount = 0;

        private Cell[,] Board = new Cell[DEFAULT_WINDOW_WIDTH/10, DEFAULT_WINDOW_HEIGHT/10];

        public GameOfLifeEngine() : base(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, WINDOW_TITLE, Color.Red) {}

        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < Board.GetLength(0); i++)
            {
                for (int j = 0; j < Board.GetLength(1); j++)
                {
                    Window.Draw(Board[i, j].Draw());
                }
            }
        }

        public override void Initialize()
        {
            InitBoard();
        }

        private void InitBoard()
        {
            bool trigger;

            for (int i = 0; i < Board.GetLength(0); i++)
            {
                for(int j = 0; j < Board.GetLength(1); j++)
                {
                    trigger = (new Random().Next(3) != 0) ? false : true;
                    Board[i, j] = new Cell(i, j, trigger);
                }
            }
        }

        public override void LoadContent()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            TimeCount++;

            if (TimeCount == 7)
            {
                TimeCount = 0;
                Cell[,] NewBoard = new Cell[DEFAULT_WINDOW_WIDTH / 10, DEFAULT_WINDOW_HEIGHT / 10];

                List<Cell> cells;

                for (int i = 0; i < Board.GetLength(0); i++)
                {
                    for (int j = 0; j < Board.GetLength(1); j++)
                    {
                        cells = GetCellsToLookAt(i, j);

                        NewBoard[i, j] = Board[i, j].Update(cells);
                    }
                }
                Board = NewBoard;
            }
        }

        private List<Cell> GetCellsToLookAt(int X, int Y)
        {
            List<Cell> cells = new List<Cell>();

            for(int i = -1;  i <= 1; i++) 
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (!(i == 0 && j == 0)
                       && X + i >= 0
                       && X + i < (DEFAULT_WINDOW_WIDTH / 10) - 1
                       && Y + j >= 0
                       && Y + j < (DEFAULT_WINDOW_HEIGHT / 10) - 1)
                    {
                        cells.Add(Board[X + i, Y + j]);
                    }
                }
            }
            return cells;
        }
    }
}
