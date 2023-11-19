using System;
using GameOfLife.Game;

namespace FourWays
{
    class Program
    {
        static void Main(string[] args)
        {
            GameOfLifeEngine gameOfLifeEngine = new GameOfLifeEngine();
            gameOfLifeEngine.Run();
        }
    }
}
