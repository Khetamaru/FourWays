using FourWays.Loop;
using SFML.Graphics;
using SFML.System;
using System;

namespace FourWays.Game.Objects.Graphs
{
    internal class Legend
    {
        internal const string CONSOLE_FONT_PATH = "./fonts/arial.ttf";
        internal static Font ConsoleFont;

        internal GameLoop Parent;
        internal Vector2f Position;
        internal Vector2f Size;
        internal Color FontColor;

        public Legend(GameLoop parent, Vector2f position, Color fontColor)
        {
            ConsoleFont = new Font(CONSOLE_FONT_PATH);
            Parent = parent;
            FontColor = fontColor;

            Size = new Vector2f(150, 25 * (Enum.GetNames(typeof(DeathGraph.DeathColor)).Length));
            Position = new Vector2f(position.X, position.Y);
        }

        internal void DrawDataTab()
        {
            DrawGraphBackground();
            DrawLines();
        }

        private void DrawGraphBackground()
        {
            RectangleShape background = new RectangleShape(Size);
            background.Position = new Vector2f(Position.X - 5f, Position.Y - 5f);

            DrawBorder(background);
        }

        private void DrawBorder(RectangleShape background)
        {
            background.FillColor = Color.Black;

            RectangleShape backgroundBorderX = new RectangleShape(new Vector2f(background.Size.X + 5f, 5f));
            backgroundBorderX.Position = new Vector2f(background.Position.X, background.Position.Y + background.Size.Y);
            backgroundBorderX.FillColor = Color.White;

            RectangleShape backgroundBorderY = new RectangleShape(new Vector2f(5f, background.Size.Y + 5f));
            backgroundBorderY.Position = new Vector2f(background.Position.X + background.Size.X, background.Position.Y);
            backgroundBorderY.FillColor = Color.White;

            RectangleShape backgroundBorderXN = new RectangleShape(new Vector2f(background.Size.X + 10f, 5f));
            backgroundBorderXN.Position = new Vector2f(background.Position.X - 5f, background.Position.Y - 5f);
            backgroundBorderXN.FillColor = Color.White;

            RectangleShape backgroundBorderYN = new RectangleShape(new Vector2f(5f, background.Size.Y + 10f));
            backgroundBorderYN.Position = new Vector2f(background.Position.X - 5f, background.Position.Y - 5f);
            backgroundBorderYN.FillColor = Color.White;

            Parent.Window.Draw(backgroundBorderX);
            Parent.Window.Draw(backgroundBorderY);
            Parent.Window.Draw(backgroundBorderXN);
            Parent.Window.Draw(backgroundBorderYN);
            Parent.Window.Draw(background);
        }

        private void DrawLines()
        {
            Text text;
            for (int i = 0; i < Enum.GetNames(typeof(DeathGraph.DeathColor)).Length; i++)
            {
                switch(i)
                {
                    case (int)DeathGraph.DeathColor.red:

                        text = new Text("Red Car : Normal", ConsoleFont, 14);
                        text.Position = new Vector2f(Position.X, Position.Y + (25 * i));
                        text.FillColor = Color.Red;
                        Parent.Window.Draw(text);
                        break;
                    case (int)DeathGraph.DeathColor.blue:

                        text = new Text("Blue Car : Normal", ConsoleFont, 14);
                        text.Position = new Vector2f(Position.X, Position.Y + (25 * i));
                        text.FillColor = Color.Cyan;
                        Parent.Window.Draw(text);
                        break;
                    case (int)DeathGraph.DeathColor.green:

                        text = new Text("Green Car : Normal", ConsoleFont, 14);
                        text.Position = new Vector2f(Position.X, Position.Y + (25 * i));
                        text.FillColor = Color.Green;
                        Parent.Window.Draw(text);
                        break;
                    case (int)DeathGraph.DeathColor.grey:

                        text = new Text("Grey Car : Normal", ConsoleFont, 14);
                        text.Position = new Vector2f(Position.X, Position.Y + (25 * i));
                        text.FillColor = Color.White;
                        Parent.Window.Draw(text);
                        break;
                    case (int)DeathGraph.DeathColor.pink:

                        text = new Text("Pink Car : Normal", ConsoleFont, 14);
                        text.Position = new Vector2f(Position.X, Position.Y + (25 * i));
                        text.FillColor = Color.Magenta;
                        Parent.Window.Draw(text);
                        break;
                    case (int)DeathGraph.DeathColor.white:

                        text = new Text("White Car : Normal", ConsoleFont, 14);
                        text.Position = new Vector2f(Position.X, Position.Y + (25 * i));
                        text.FillColor = Color.White;
                        Parent.Window.Draw(text);
                        break;
                    case (int)DeathGraph.DeathColor.yellow:

                        text = new Text("Yellow Car : Normal", ConsoleFont, 14);
                        text.Position = new Vector2f(Position.X, Position.Y + (25 * i));
                        text.FillColor = Color.Yellow;
                        Parent.Window.Draw(text);
                        break;
                    case (int)DeathGraph.DeathColor.purple:

                        text = new Text("Purple Car : Normal", ConsoleFont, 14);
                        text.Position = new Vector2f(Position.X, Position.Y + (25 * i));
                        text.FillColor = Color.Magenta;
                        Parent.Window.Draw(text);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
