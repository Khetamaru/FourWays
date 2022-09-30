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
        public const float TIME_UNTIL_UPDATE = 2f;

        float totalTimeBeforeUpdate = 0f;
        float previousTimeElapsed = 0f;
        float deltaTime;
        float totalTimeElapsed;

        RoadLight RoadLightLeft;

        Clock clock = new Clock();

        public const string GREEN_LIGHT_PATH = "./Ressources/traffic-lights-green.png";
        public const string ORANGE_LIGHT_PATH = "./Ressources/traffic-lights-orange.png";
        public const string RED_LIGHT_PATH = "./Ressources/traffic-lights-red.png";

        private Texture RoadLightGreen { get; set; }
        private Texture RoadLightOrange { get; set; }
        private Texture RoadLightRed { get; set; }

        public RectangleShape Image { get; private set; }

        public Car.Direction direction;

        private State actualState;
        public State state 
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

        public enum State
        {
            Green,
            Orange,
            Red
        }

        public RectangleShape StopArea;

        public RoadLight(Vector2f position, Car.Direction direction, RectangleShape StopArea, State startLight) 
        {
            LoadContent();
            Initialize(position, direction, StopArea, startLight);
        }

        public void LoadContent()
        {
            RoadLightGreen = new Texture(new Image(GREEN_LIGHT_PATH));
            RoadLightOrange = new Texture(new Image(ORANGE_LIGHT_PATH));
            RoadLightRed = new Texture(new Image(RED_LIGHT_PATH));
        }

        private void Initialize(Vector2f position, Car.Direction direction, RectangleShape stopArea, State startLight)
        {
            Image = new RectangleShape();
            Image.Position = position;
            Image.Size = new Vector2f(48f, 72f);

            StopArea = stopArea;
            StopArea.FillColor = Color.Cyan;

            state = startLight;
            this.direction = direction;

            switch (direction)
            {
                case Car.Direction.left:

                    Image.Rotation = -90;
                    break;

                case Car.Direction.right:

                    Image.Rotation = 90;
                    break;
            }
        }

        public override void Update()
        {
            if (RoadLightLeft.state != State.Green && state == State.Red || state != State.Red)
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
                        case State.Green:
                            state = State.Orange;
                            break;

                        case State.Orange:
                            state = State.Red;
                            break;

                        case State.Red:
                            state = State.Green;
                            break;
                    }
                }
            }
            else
            {
                totalTimeBeforeUpdate = 0f;
            }
        }

        private void ApplyTexture(State state)
        {
            switch (state)
            {
                case State.Green:

                    Image.Texture = RoadLightGreen;
                    break;

                case State.Orange:

                    Image.Texture = RoadLightOrange;
                    break;

                case State.Red:

                    Image.Texture = RoadLightRed;
                    break;

            }
        }

        public void AssignRoadLightLeft(RoadLight roadLight)
        {
            RoadLightLeft = roadLight;
        }
    }
}
