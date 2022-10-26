using System;
using FourWays.Loop;
using SFML.Graphics;
using SFML.System;

namespace FourWays.Game
{
    public static class DebugUtility
    {
        internal const string CONSOLE_FONT_PATH = "./fonts/arial.ttf";
        internal static Font consoleFont;

        internal static void LoadContent()
        {
            consoleFont = new Font(CONSOLE_FONT_PATH);
        }

        internal static void DrawPerformanceData(GameLoop gameLoop, Color fontColor)
        {
            if (consoleFont == null)
            {
                return;
            }

            DrawPerformanceDataBackgroud(gameLoop);
            DrawPerformanceDataInfos(gameLoop, fontColor);
        }

        private static void DrawPerformanceDataBackgroud(GameLoop gameLoop)
        {
            RectangleShape background = new RectangleShape(new Vector2f(190f, 90f));
            background.Position = new Vector2f(0f, 0f);
            background.FillColor = Color.Blue;

            RectangleShape backgroundBorderX = new RectangleShape(new Vector2f(background.Size.X + 5f, 5f));
            backgroundBorderX.Position = new Vector2f(0f, background.Size.Y);
            backgroundBorderX.FillColor = Color.Red;

            RectangleShape backgroundBorderY = new RectangleShape(new Vector2f(5f, background.Size.Y + 5f));
            backgroundBorderY.Position = new Vector2f(background.Size.X, 0f);
            backgroundBorderY.FillColor = Color.Red;

            gameLoop.Window.Draw(backgroundBorderX);
            gameLoop.Window.Draw(backgroundBorderY);
            gameLoop.Window.Draw(background);
        }

        private static void DrawPerformanceDataInfos(GameLoop gameLoop, Color fontColor)
        {
            string totalTimeElapsedStr = (Math.Round(Time.FromSeconds(gameLoop.GameTime.TotalTimeElapsed).AsSeconds() / 60, 0) + 
                                         "m:" +
                                         Math.Round(Time.FromSeconds(gameLoop.GameTime.TotalTimeElapsed).AsSeconds() % 60, 0) +
                                         "s")
                                         .ToString();
            string deltaTimeStr =        (Math.Round(Time.FromSeconds(gameLoop.GameTime.DeltaTime).AsSeconds() / 60, 0) +
                                         "m:" +
                                         Math.Round(Time.FromSeconds(gameLoop.GameTime.DeltaTime).AsSeconds() % 60, 0) +
                                         "s" +
                                         Math.Round((float)Time.FromSeconds(gameLoop.GameTime.DeltaTime).AsMilliseconds() % 100, 0) +
                                         "mls")
                                         .ToString();
            string fpsStr = (1f / gameLoop.GameTime.DeltaTime).ToString("0.00");

            Text text = new Text("Time Elapsed : " + totalTimeElapsedStr, consoleFont, 14);
            text.Position = new Vector2f(4f, 8f);
            text.FillColor = fontColor;

            Text textB = new Text("Refresh Time : " + deltaTimeStr, consoleFont, 14);
            textB.Position = new Vector2f(4f, 28f);
            textB.FillColor = fontColor;

            Text textC = new Text("FPS : " + fpsStr, consoleFont, 14);
            textC.Position = new Vector2f(4f, 48f);
            textC.FillColor = fontColor;

            Text textD = new Text("Death Counter : " + (gameLoop as FourWaysSimulator).DEATH_COUNTER, consoleFont, 14);
            textD.Position = new Vector2f(4f, 68f);
            textD.FillColor = fontColor;

            gameLoop.Window.Draw(text);
            gameLoop.Window.Draw(textB);
            gameLoop.Window.Draw(textC);
            gameLoop.Window.Draw(textD);
        }
    }
}
