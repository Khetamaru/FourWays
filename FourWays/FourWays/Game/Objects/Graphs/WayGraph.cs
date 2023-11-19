using FourWays.Loop;
using SFML.Graphics;
using SFML.System;
using System;

namespace FourWays.Game.Objects.Graphs
{
    internal class WayGraph
    {
        internal const string CONSOLE_FONT_PATH = "./fonts/arial.ttf";
        internal static Font ConsoleFont;

        internal GameLoop Parent;
        internal Vector2f Position;
        internal Vector2f Size;
        internal Color FontColor;

        internal int[] ResultTab;

        public WayGraph(GameLoop parent, Vector2f position, Color fontColor)
        {
            ConsoleFont = new Font(CONSOLE_FONT_PATH);
            Parent = parent;
            FontColor = fontColor;

            ResultTab = new int[2];
            for (int i = 0; i < ResultTab.Length; i++) ResultTab[i] = 0;

            Size = new Vector2f(150, 25 * (ResultTab.Length + 1));
            Position = new Vector2f(position.X - Size.X, position.Y);
        }

        internal void AddResultCounter(bool sucess)
        {
            if (sucess) ResultTab[0]++;
            else ResultTab[1]++;
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
            Text text1 = new Text("Good Way : " + ResultTab[0], ConsoleFont, 14);
            text1.Position = new Vector2f(Position.X, Position.Y);
            text1.FillColor = FontColor;
            Parent.Window.Draw(text1);

            Text text2 = new Text("Wrong Way : " + ResultTab[1], ConsoleFont, 14);
            text2.Position = new Vector2f(Position.X, Position.Y + 25);
            text2.FillColor = FontColor;
            Parent.Window.Draw(text2);

            Text text3 = new Text("Sucess Rate : " + (ResultTab[0] > 0 ? Math.Round((double)ResultTab[0] / (ResultTab[0] + ResultTab[1]) * 100, 2) : "100") + "%", ConsoleFont, 14);
            text3.Position = new Vector2f(Position.X, Position.Y + 50);
            text3.FillColor = FontColor;
            Parent.Window.Draw(text3);
        }
    }
}
