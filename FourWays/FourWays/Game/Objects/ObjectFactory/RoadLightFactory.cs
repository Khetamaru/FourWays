using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace FourWays.Game.Objects.ObjectFactory
{
    public class RoadLightFactory
    {
        private const uint DEFAULT_WINDOW_WIDTH = 1280;
        private const uint DEFAULT_WINDOW_HEIGHT = 960;

        private const uint GROUND_WIDTH = (DEFAULT_WINDOW_WIDTH - 110) / 2;
        private const uint GROUND_HEIGHT = (DEFAULT_WINDOW_HEIGHT - 100) / 2;

        public RoadLightFactory() { }

        internal Dictionary<Direction, RoadLight> RoadLightInit()
        {
            Dictionary<Direction, RoadLight> roadLights = new Dictionary<Direction, RoadLight>();

            roadLights.Add(Direction.right, RoadLightCreation(true, Direction.right, GROUND_WIDTH - 10f,  GROUND_HEIGHT + 105f, GROUND_WIDTH - 60f,  GROUND_HEIGHT + 55f,   GROUND_WIDTH - 120f, GROUND_HEIGHT + 55f));
            roadLights.Add(Direction.down, RoadLightCreation(false, Direction.down,  GROUND_WIDTH - 60f,  GROUND_HEIGHT - 80f,  GROUND_WIDTH + 5f,   GROUND_HEIGHT - 60f,   GROUND_WIDTH + 5f,   GROUND_HEIGHT - 120f));
            roadLights.Add(Direction.up, RoadLightCreation(  false, Direction.up,    GROUND_WIDTH + 110f, GROUND_HEIGHT + 105f, GROUND_WIDTH + 55f,  GROUND_HEIGHT + +100f, GROUND_WIDTH + 55f,  GROUND_HEIGHT + +160f));
            roadLights.Add(Direction.left, RoadLightCreation( true, Direction.left,  GROUND_WIDTH + 110f, GROUND_HEIGHT - 10f,  GROUND_WIDTH + 100f, GROUND_HEIGHT + 5f,    GROUND_WIDTH + 160f, GROUND_HEIGHT + 5f));

            RoadLightAlignement(roadLights);

            return roadLights;
        }

        private RoadLight RoadLightCreation(bool horizontal, Direction direction, float PositionX, float PositionY, float PositionXStop, float PositionYStop, float PositionXDecelarate, float PositionYDecelarate)
        {
            float x;
            float y;
            RoadLightState roadLightState;

            if (horizontal) { x = 60f; y = 40f; roadLightState = RoadLightState.Green; }
            else {            x = 40f; y = 60f; roadLightState = RoadLightState.Red; }

            RectangleShape StopArea = new RectangleShape(new Vector2f(x, y));
            RectangleShape DecelerateArea = new RectangleShape(new Vector2f(x, y));

            StopArea.Position = new Vector2f(PositionXStop, PositionYStop);
            DecelerateArea.Position = new Vector2f(PositionXDecelarate, PositionYDecelarate);

            return new RoadLight(new Vector2f(PositionX, PositionY), direction, StopArea, DecelerateArea, roadLightState);
        }

        private void RoadLightAlignement(Dictionary<Direction, RoadLight> roadLights)
        {
            roadLights.TryGetValue(Direction.left, out RoadLight tempLeft);
            roadLights.TryGetValue(Direction.right, out RoadLight tempRight);
            roadLights.TryGetValue(Direction.up, out RoadLight tempUp);
            roadLights.TryGetValue(Direction.down, out RoadLight tempDown);

            tempLeft.AssignRoadLightLeft(tempUp);
            tempUp.AssignRoadLightLeft(tempRight);
            tempRight.AssignRoadLightLeft(tempDown);
            tempDown.AssignRoadLightLeft(tempLeft);
        }
    }
}
