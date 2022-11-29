using FourWays.Loop;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourWays.Game.Objects.Graphs
{
    public class DeathGraph
    {
        internal const string CONSOLE_FONT_PATH = "./fonts/arial.ttf";
        internal static Font ConsoleFont;

        internal GameLoop Parent;
        internal Vector2f Position;
        internal Vector2f Size;
        internal Color FontColor;

        internal int[] DeathCounters;

        public enum DeathColor
        {
            red,
            blue,
            green,
            grey,
            purple,
            yellow,
            pink,
            white
        }

        public DeathGraph(GameLoop parent, Vector2f position, Color fontColor)
        {
            ConsoleFont = new Font(CONSOLE_FONT_PATH);
            Parent = parent;
            FontColor = fontColor;

            InitDeathCounters();

            Position = new Vector2f(position.X - 50 * DeathCounters.Length - 25, position.Y);
            Size = new Vector2f(50 * DeathCounters.Length + 30f, 310f);
        }

        private void InitDeathCounters()
        {
            DeathCounters = new int[Enum.GetNames(typeof(DeathColor)).Length];
            for (int i = 0; i < DeathCounters.Length; i++) DeathCounters[i] = 0;
        }

        internal void AddDeathCounter(DeathColor deathColor)
        {
            DeathCounters[(int)deathColor]++;
        }

        internal void DrawDataTab()
        {
            DrawBackground();
            DrawTitle();
            int j = 0;
            foreach (int i in DeathCounters)
            {
                DrawColorName(j);
                DrawGraph(i, j);
                DrawPercentage(i, j);

                j++;
            }
            DrawDeathPerSecond();
        }

        private void DrawBackground()
        {
            DrawTitleBackground();
            DrawGraphBackground();
            DrawDeathPerSecondBackground();
        }

        private void DrawTitleBackground()
        {
            RectangleShape background = new RectangleShape(new Vector2f(50 * DeathCounters.Length + 10f, 30f));
            background.Position = new Vector2f(Position.X - 5f, Position.Y - 5f);

            DrawBorder(background);
        }

        private void DrawGraphBackground()
        {
            RectangleShape background = new RectangleShape(new Vector2f(50 * DeathCounters.Length + 10f, 255f));
            background.Position = new Vector2f(Position.X - 5f, Position.Y - 5f + 30f);

            DrawBorder(background);
        }

        private void DrawDeathPerSecondBackground()
        {
            RectangleShape background = new RectangleShape(new Vector2f(50 * DeathCounters.Length + 10f, 30f));
            background.Position = new Vector2f(Position.X - 5f, Position.Y - 5f + 285f);

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

        private void DrawTitle()
        {
            Text text = new Text("Crash Total : " + DeathTotal()/2, ConsoleFont, 14);
            text.Position = new Vector2f(Position.X, Position.Y);
            text.FillColor = FontColor;

            Parent.Window.Draw(text);
        }

        private void DrawColorName(int index)
        {
            DeathColor color = (DeathColor)index;

            Text text = new Text(color.ToString(), ConsoleFont, 14);
            text.Position = new Vector2f(Position.X + (50 * index), Position.Y + 30f);
            text.FillColor = FontColor;

            Parent.Window.Draw(text);
        }

        private void DrawGraph(int i, int index)
        {
            int sizeY = DeathTotal() > 0 ? int.Parse(Math.Round((double)i / DeathTotal() * 200, 0).ToString()) : 0;

            RectangleShape column = new RectangleShape(new Vector2f(30f, sizeY));
            column.Position = new Vector2f(Position.X + (50 * index), Position.Y + 20f + 30f);
            column.FillColor = GetColor((DeathColor)index);

            Parent.Window.Draw(column);
        }

        private void DrawPercentage(int i, int index)
        {
            int sizeY = DeathTotal() > 0 ? int.Parse(Math.Round((double)i / DeathTotal() * 200, 0).ToString()) : 0;

            Text text = new Text(sizeY/2 + "%", ConsoleFont, 14);
            text.Position = new Vector2f(Position.X + (50 * index), Position.Y + sizeY + 25f + 30f);
            text.FillColor = FontColor;

            Parent.Window.Draw(text);
        }

        private void DrawDeathPerSecond()
        {
            Text text = new Text("Min / Death : " + Math.Round((DeathTotal() > 0 ? (Parent.GameTime.TotalTimeElapsed/60)/(DeathTotal()/2) : 0),2), ConsoleFont, 14);
            text.Position = new Vector2f(Position.X, Position.Y + 285f);
            text.FillColor = FontColor;

            Parent.Window.Draw(text);
        }

        private Color GetColor(DeathColor i)
        {
            return i switch
            {
                DeathColor.red => Color.Red,
                DeathColor.blue => Color.Blue,
                DeathColor.green => Color.Green,
                DeathColor.pink => Color.Magenta,
                DeathColor.purple => Color.Magenta,
                DeathColor.white => Color.White,
                DeathColor.grey => Color.White,
                DeathColor.yellow => Color.Yellow,
                _ => Color.White
            };
        }

        internal int DeathTotal()
        {
            int result = 0;

            foreach (int i in DeathCounters)
            {
                result += i;
            }
            return result;
        }
    }
}
