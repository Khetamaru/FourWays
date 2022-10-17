using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourWays.Game.Objects
{
    public class RoadLight : GameObject
    {
        internal const float TIME_UNTIL_UPDATE = 2f;

        float totalTimeBeforeUpdate = 0f;
        float previousTimeElapsed = 0f;
        float deltaTime;
        float totalTimeElapsed;

        RoadLight RoadLightLeft;

        Clock clock = new Clock();

        internal const string GREEN_LIGHT_PATH = "./Ressources/traffic-lights-green.png";
        internal const string ORANGE_LIGHT_PATH = "./Ressources/traffic-lights-orange.png";
        internal const string RED_LIGHT_PATH = "./Ressources/traffic-lights-red.png";

        private Texture RoadLightGreen;
        private Texture RoadLightOrange;
        private Texture RoadLightRed;

        internal RectangleShape Image { get; private set; }

        internal Direction direction;

        private RoadLightState actualState;
        internal RoadLightState state 
        { 
            get
            {
                return actualState;
            }
            private set
            {
                actualState = value;
                ApplyTexture(value);
            } 
        }

        internal RectangleShape StopArea;
        internal RectangleShape DecelerateArea;

        public RoadLight(Vector2f position, Direction direction, RectangleShape StopArea, RectangleShape DecelerateArea, RoadLightState startLight) 
        {
            LoadContent();
            Initialize(position, direction, StopArea, DecelerateArea, startLight);
        }

        internal void LoadContent()
        {
            RoadLightGreen = new Texture(new Image(GREEN_LIGHT_PATH));
            RoadLightOrange = new Texture(new Image(ORANGE_LIGHT_PATH));
            RoadLightRed = new Texture(new Image(RED_LIGHT_PATH));
        }

        private void Initialize(Vector2f position, Direction direction, RectangleShape stopArea, RectangleShape decelerateArea, RoadLightState startLight)
        {
            Image = new RectangleShape();
            Image.Position = position;
            Image.Size = new Vector2f(48f, 72f);

            StopArea = stopArea;
            DecelerateArea = decelerateArea;
            StopArea.FillColor = Color.Cyan;

            state = startLight;
            this.direction = direction;

            switch (direction)
            {
                case Direction.left:

                    Image.Rotation = -90;
                    break;

                case Direction.right:

                    Image.Rotation = 90;
                    break;
            }
        }

        internal override void Update()
        {
            if (RoadLightLeft.state != RoadLightState.Green && state == RoadLightState.Red || state != RoadLightState.Red)
            {
                totalTimeElapsed = clock.ElapsedTime.AsSeconds();
                deltaTime = totalTimeElapsed - previousTimeElapsed;
                previousTimeElapsed = totalTimeElapsed;

                totalTimeBeforeUpdate += deltaTime;

                if (totalTimeBeforeUpdate >= TIME_UNTIL_UPDATE)
                {
                    totalTimeBeforeUpdate = 0f;

                    switch (state)
                    {
                        case RoadLightState.Green:
                            state = RoadLightState.Orange;
                            break;

                        case RoadLightState.Orange:
                            state = RoadLightState.Red;
                            break;

                        case RoadLightState.Red:
                            state = RoadLightState.Green;
                            break;
                    }
                }
            }
            else
            {
                totalTimeBeforeUpdate = 0f;
            }
        }

        private void ApplyTexture(RoadLightState state)
        {
            switch (state)
            {
                case RoadLightState.Green:

                    Image.Texture = RoadLightGreen;
                    break;

                case RoadLightState.Orange:

                    Image.Texture = RoadLightOrange;
                    break;

                case RoadLightState.Red:

                    Image.Texture = RoadLightRed;
                    break;

            }
        }

        internal void AssignRoadLightLeft(RoadLight roadLight)
        {
            RoadLightLeft = roadLight;
        }
    }

    public enum RoadLightState
    {
        Green,
        Orange,
        Red
    }
}
