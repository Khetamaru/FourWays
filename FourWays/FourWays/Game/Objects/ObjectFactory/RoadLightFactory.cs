using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace FourWays.Game.Objects.ObjectFactory
{
    public class RoadLightFactory
    {
        private const uint DEFAULT_WINDOW_WIDTH = 1280;
        private const uint DEFAULT_WINDOW_HEIGHT = 960;

        private const uint GROUND_WIDTH = (DEFAULT_WINDOW_WIDTH - 110) / 2;
        private const uint GROUND_HEIGHT = (DEFAULT_WINDOW_HEIGHT - 100) / 2;

        private Action<RectangleShape> ExternalDrawFunction;
        private bool BreakPointHighlightTrigger;

        public RoadLightFactory(Action<RectangleShape> ExternalDrawFunction, bool BreakPointHighlightTrigger)
        {
            this.ExternalDrawFunction = ExternalDrawFunction;
            this.BreakPointHighlightTrigger = BreakPointHighlightTrigger;
        }

        internal Dictionary<Direction, RoadLight> RoadLightInit()
        {
            Dictionary<Direction, RoadLight> roadLights = new Dictionary<Direction, RoadLight>();

            roadLights.Add(Direction.right, RoadLightCreation(true, Direction.right, GROUND_WIDTH - 10f,  GROUND_HEIGHT + 105f, GROUND_WIDTH,        GROUND_HEIGHT + 65f));
            roadLights.Add(Direction.down, RoadLightCreation(false, Direction.down,  GROUND_WIDTH - 60f,  GROUND_HEIGHT - 80f,  GROUND_WIDTH + 15f,   GROUND_HEIGHT));
            roadLights.Add(Direction.up, RoadLightCreation(  false, Direction.up,    GROUND_WIDTH + 110f, GROUND_HEIGHT + 105f, GROUND_WIDTH + 65f,  GROUND_HEIGHT + +100f));
            roadLights.Add(Direction.left, RoadLightCreation( true, Direction.left,  GROUND_WIDTH + 110f, GROUND_HEIGHT - 10f,  GROUND_WIDTH + 100f, GROUND_HEIGHT + 15f));

            RoadLightAlignement(roadLights);

            return roadLights;
        }

        private RoadLight RoadLightCreation(bool horizontal, Direction direction, float PositionX, float PositionY, float PositionXStop, float PositionYStop)
        {
            float x;
            float y;
            RoadLightState roadLightState;

            if (horizontal) { x = 2f; y = 20f; roadLightState = RoadLightState.Green; }
            else {            x = 20f; y = 2f; roadLightState = RoadLightState.Red; }

            RectangleShape StopArea = new RectangleShape(new Vector2f(x, y));

            StopArea.Position = new Vector2f(PositionXStop, PositionYStop);

            return new RoadLight(new Vector2f(PositionX, PositionY), direction, StopArea, roadLightState, ExternalDrawFunction, BreakPointHighlightTrigger);
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
