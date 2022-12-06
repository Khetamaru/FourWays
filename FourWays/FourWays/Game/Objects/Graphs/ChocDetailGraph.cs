using FourWays.Game.Objects.ObjectFactory;
using FourWays.Loop;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace FourWays.Game.Objects.Graphs
{
    public class ChocDetailGraph
    {
        internal const string CONSOLE_FONT_PATH = "./fonts/arial.ttf";
        internal static Font ConsoleFont;

        internal GameLoop Parent;
        internal Vector2f Position;
        internal Vector2f Size;
        internal Color FontColor;

        internal Dictionary<KeyValuePair<CarColor, CarColor>, int> KeyValueList;

        public ChocDetailGraph(GameLoop parent, Vector2f position, Color fontColor)
        {
            ConsoleFont = new Font(CONSOLE_FONT_PATH);
            Parent = parent;
            FontColor = fontColor;

            Position = new Vector2f(position.X, position.Y);
            Size = new Vector2f(510f, 410f);

            InitKeyValueList();
        }

        private void InitKeyValueList()
        {
            KeyValueList = new Dictionary<KeyValuePair<CarColor, CarColor>, int>();
        }

        internal void AddKeyValuePair(KeyValuePair<CarColor, CarColor> pair)
        {
            bool trigger = false;
            KeyValuePair<CarColor, CarColor> i = new KeyValuePair<CarColor, CarColor>();

            foreach(KeyValuePair<KeyValuePair<CarColor, CarColor>, int> keyValuePair in KeyValueList)
            {
                if((pair.Key == keyValuePair.Key.Key &&
                    pair.Value == keyValuePair.Key.Value) ||
                   (pair.Key == keyValuePair.Key.Value &&
                    pair.Value == keyValuePair.Key.Key))
                {
                    trigger = true;
                    i = keyValuePair.Key;
                }
            }
            if (trigger)
            {
                KeyValueList.TryGetValue(i, out int j);
                KeyValueList.Remove(i);
                KeyValueList.Add(i, j + 1);
            }
            else
            {
                KeyValueList.Add(pair, 1);
            }
        }

        internal void DrawDataTab()
        {
            DrawBackground();

            Vector2f elementPosition = new Vector2f (Position.X, Position.Y - 22);

            foreach (KeyValuePair<KeyValuePair<CarColor, CarColor>, int> keyValuePair in KeyValueList)
            {
                elementPosition = DrawChocDetails(keyValuePair, elementPosition);
            }
        }

        private void DrawBackground()
        {
            RectangleShape background = new RectangleShape(new Vector2f(Size.X, Size.Y));
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

        private Vector2f DrawChocDetails(KeyValuePair<KeyValuePair<CarColor, CarColor>, int> keyValuePair, Vector2f elementPosition)
        {
            float delta = 22f;
            uint textZise = 15;

            Vector2f position = new Vector2f(elementPosition.X, elementPosition.Y + delta);

            if (position.Y + delta > Position.Y + Size.Y)
            {
                position.X = Position.X + 260f;
                position.Y = Position.Y;
            }

            Text text1 = new Text(keyValuePair.Key.Key.ToString(), ConsoleFont, textZise);
            text1.Position = new Vector2f(position.X + 10f, position.Y);
            text1.FillColor = GetColor(keyValuePair.Key.Key);
            Parent.Window.Draw(text1);

            Text text2 = new Text("-", ConsoleFont, 14);
            text2.Position = new Vector2f(text1.Position.X + 70f, position.Y);
            text2.FillColor = FontColor;
            Parent.Window.Draw(text2);

            Text text3 = new Text(keyValuePair.Key.Value.ToString(), ConsoleFont, textZise);
            text3.Position = new Vector2f(text1.Position.X + 105f, position.Y);
            text3.FillColor = GetColor(keyValuePair.Key.Value);
            Parent.Window.Draw(text3);

            Text text4 = new Text("=>  " + Math.Round((double)keyValuePair.Value / 2, 0).ToString(), ConsoleFont, textZise);
            text4.Position = new Vector2f(text1.Position.X + 170f, position.Y);
            text4.FillColor = FontColor;
            Parent.Window.Draw(text4);

            return position;
        }

        private Color GetColor(CarColor i)
        {
            return i switch
            {
                CarColor.red => Color.Red,
                CarColor.blue => Color.Cyan,
                CarColor.green => Color.Green,
                CarColor.pink => Color.Magenta,
                CarColor.purple => Color.Magenta,
                CarColor.white => Color.White,
                CarColor.grey => Color.White,
                CarColor.yellow => Color.Yellow,
                _ => Color.White
            };
        }
    }
}
