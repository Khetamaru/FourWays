using System;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using FourWays.Loop;

namespace FourWays.Game
{
    public class TutoAme : GameLoop
    {
        public const uint DEFAULT_WINDOW_WIDTH = 640;
        public const uint DEFAULT_WINDOW_HEIGHT = 480;

        public const string WINDOW_TITLE = "TutoAme";

        public TutoAme() : base(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, WINDOW_TITLE, Color.Black)
        {
        }

        public override void LoadContent()
        {
            DebugUtility.LoadContent();
        }

        public override void Initialize()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            DebugUtility.DrawPerformanceData(this, Color.White);
        }
    }
}
