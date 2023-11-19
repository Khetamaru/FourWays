using System;
using Elevator.Game;

namespace FourWays
{
    class Program
    {
        static void Main(string[] args)
        {
            ElevatorSimulator elevator = new ElevatorSimulator();
            elevator.Run();
        }
    }
}