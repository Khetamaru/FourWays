using Elevator.Game.Objects;
using FourWays.Loop;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevator.Game
{
    internal class ElevatorSimulator : GameLoop
    {
        private const uint DEFAULT_WINDOW_WIDTH = 1280;
        private const uint DEFAULT_WINDOW_HEIGHT = 960;

        private const string WINDOW_TITLE = "Elevator";
        public const string CONSOLE_FONT_PATH = "./fonts/arial.ttf";
        public static Font consoleFont;

        private const int MAX_FLOOR = 4;
        private const int LIFT_NUMBER = 1;

        private Lift[] LiftCollection;

        private Text FloorFive;
        private Text FloorFour;
        private Text FloorThree;
        private Text FloorTwo;
        private Text FloorOne;
        private Text Floor;
        const int FloorX = 50;
        const int FloorY = 150;

        public ElevatorSimulator() : base(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, WINDOW_TITLE, Color.Black)
        {
            consoleFont = new Font(CONSOLE_FONT_PATH);
            LiftCollection = new Lift[LIFT_NUMBER];
            LiftCollection[0] = new Lift(500, 500, consoleFont, FloorY);

            LiftCollection[0].Objectif = 4;
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (Lift lift in LiftCollection)
            {
                lift.Draw(this);
            }
            FloorFive = new Text("ETAGE 5", consoleFont, 20);
            FloorFive.Position = new Vector2f(FloorX, 1 * FloorY);
            FloorFive.FillColor = Color.White;
            Window.Draw(FloorFive);


            FloorFour = new Text("ETAGE 4", consoleFont, 20);
            FloorFour.Position = new Vector2f(FloorX, 2 * FloorY);
            FloorFour.FillColor = Color.White;
            Window.Draw(FloorFour);


            FloorThree = new Text("ETAGE 3", consoleFont, 20);
            FloorThree.Position = new Vector2f(FloorX, 3 * FloorY);
            FloorThree.FillColor = Color.White;
            Window.Draw(FloorThree);


            FloorTwo = new Text("ETAGE 2", consoleFont, 20);
            FloorTwo.Position = new Vector2f(FloorX, 4 * FloorY);
            FloorTwo.FillColor = Color.White;
            Window.Draw(FloorTwo);


            FloorOne = new Text("ETAGE 1", consoleFont, 20);
            FloorOne.Position = new Vector2f(FloorX, 5 * FloorY);
            FloorOne.FillColor = Color.White;
            Window.Draw(FloorOne);


            Floor = new Text("REZ DE CHAUSSE", consoleFont, 20);
            Floor.Position = new Vector2f(FloorX, 6 * FloorY);
            Floor.FillColor = Color.White;
            Window.Draw(Floor);
        }

        public override void Initialize()
        {
            
        }

        public override void LoadContent()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            foreach(Lift lift in LiftCollection)
            {
                lift.Update();
            }
        }
    }
}
