using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourWays.Game.Objects.ObjectFactory
{
    public class RoadLightFactory
    {
        private const uint DEFAULT_WINDOW_WIDTH = 1280;
        private const uint DEFAULT_WINDOW_HEIGHT = 960;

        private const uint GROUND_WIDTH = (DEFAULT_WINDOW_WIDTH - 110) / 2;
        private const uint GROUND_HEIGHT = (DEFAULT_WINDOW_HEIGHT - 100) / 2;

        public RoadLightFactory()
        {

        }

        internal Dictionary<Direction, RoadLight> RoadLightInit(Dictionary<Direction, RoadLight> roadLights)
        {
            RectangleShape rightStopArea = new RectangleShape(new Vector2f(60f, 40f));
            RectangleShape downStopArea = new RectangleShape(new Vector2f(40f, 60f));
            RectangleShape upStopArea = new RectangleShape(new Vector2f(40f, 60f));
            RectangleShape leftStopArea = new RectangleShape(new Vector2f(60f, 40f));

            RectangleShape rightDecelerateArea = new RectangleShape(new Vector2f(60f, 40f));
            RectangleShape downDecelerateArea = new RectangleShape(new Vector2f(40f, 60f));
            RectangleShape upDecelerateArea = new RectangleShape(new Vector2f(40f, 60f));
            RectangleShape leftDecelerateArea = new RectangleShape(new Vector2f(60f, 40f));

            rightStopArea.Position = new Vector2f(590f - 60f, 430f + 55f);
            downStopArea.Position = new Vector2f(590f + 5f, 430f - 60f);
            upStopArea.Position = new Vector2f(590f + 55f, 430f + +100f);
            leftStopArea.Position = new Vector2f(590f + 100f, 430f + 5f);

            rightDecelerateArea.Position = new Vector2f(590f - 120f, 430f + 55f);
            downDecelerateArea.Position = new Vector2f(590f + 5f, 430f - 120f);
            upDecelerateArea.Position = new Vector2f(590f + 55f, 430f + +160f);
            leftDecelerateArea.Position = new Vector2f(590f + 160f, 430f + 5f);

            roadLights.Add(Direction.right, new RoadLight(new Vector2f(580f, 535f), Direction.right, rightStopArea, rightDecelerateArea, RoadLightState.Green));
            roadLights.Add(Direction.down, new RoadLight(new Vector2f(530f, 350f), Direction.down, downStopArea, downDecelerateArea, RoadLightState.Red));
            roadLights.Add(Direction.up, new RoadLight(new Vector2f(700f, 535f), Direction.up, upStopArea, upDecelerateArea, RoadLightState.Red));
            roadLights.Add(Direction.left, new RoadLight(new Vector2f(700f, 420f), Direction.left, leftStopArea, leftDecelerateArea, RoadLightState.Green));

            roadLights.TryGetValue(Direction.left, out RoadLight tempLeft);
            roadLights.TryGetValue(Direction.right, out RoadLight tempRight);
            roadLights.TryGetValue(Direction.up, out RoadLight tempUp);
            roadLights.TryGetValue(Direction.down, out RoadLight tempDown);

            tempLeft.AssignRoadLightLeft(tempUp);
            tempUp.AssignRoadLightLeft(tempRight);
            tempRight.AssignRoadLightLeft(tempDown);
            tempDown.AssignRoadLightLeft(tempLeft);

            return roadLights;
        }

        private (Direction, RoadLight) RoadLightCreation()
        {
            RectangleShape rightStopArea = new RectangleShape(new Vector2f(60f, 40f));
            RectangleShape rightDecelerateArea = new RectangleShape(new Vector2f(60f, 40f));

            rightStopArea.Position = new Vector2f(590f - 60f, 430f + 55f);
            rightDecelerateArea.Position = new Vector2f(590f - 120f, 430f + 55f);

            return (Direction.right, new RoadLight(new Vector2f(580f, 535f), Direction.right, rightStopArea, rightDecelerateArea, RoadLightState.Green));
        }
    }
}
