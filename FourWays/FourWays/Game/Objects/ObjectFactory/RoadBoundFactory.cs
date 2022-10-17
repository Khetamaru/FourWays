using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourWays.Game.Objects.ObjectFactory
{
    public class RoadBoundFactory
    {
        private const uint DEFAULT_WINDOW_WIDTH = 1280;
        private const uint DEFAULT_WINDOW_HEIGHT = 960;

        private const uint GROUND_WIDTH = (DEFAULT_WINDOW_WIDTH - 110) / 2;
        private const uint GROUND_HEIGHT = (DEFAULT_WINDOW_HEIGHT - 100) / 2;

        private Texture OutRoadTexture;
        private Texture RoadCenterTexture;
        private Texture RoadHorizontalTexture;
        private Texture RoadVerticalTexture;

        public RoadBoundFactory()
        {
            LoadContent();
        }

        internal void LoadContent()
        {
            OutRoadTexture = new Texture(new Image("./Ressources/green-grass-texture_1249-15.jpg"));
            RoadCenterTexture = new Texture(new Image("./Ressources/road_center.jpg"));
            RoadHorizontalTexture = new Texture(new Image("./Ressources/road_horizontal.jpg"));
            RoadVerticalTexture = new Texture(new Image("./Ressources/road_vertical.jpg"));
        }

        internal List<RectangleShape> RoadBoundInit()
        {
            List<RectangleShape> roadBounds = new List<RectangleShape>();

            GrassSetUp(roadBounds);
            RoadSetUp(roadBounds);

            return roadBounds;
        }

        internal void GrassSetUp(List<RectangleShape> roadBounds)
        {
            roadBounds.Add(ShapeCreator(GROUND_WIDTH, GROUND_HEIGHT, 0f, 0f, OutRoadTexture));

            roadBounds.Add(ShapeCreator(GROUND_WIDTH, GROUND_HEIGHT, 0f, GROUND_HEIGHT + 100f, OutRoadTexture));

            roadBounds.Add(ShapeCreator(GROUND_WIDTH, GROUND_HEIGHT, GROUND_WIDTH + 100f, 0f, OutRoadTexture));

            roadBounds.Add(ShapeCreator(GROUND_WIDTH, GROUND_HEIGHT, GROUND_WIDTH + 100f, GROUND_HEIGHT + 100f, OutRoadTexture));
        }

        internal void RoadSetUp(List<RectangleShape> roadBounds)
        {
            roadBounds.Add(ShapeCreator(100f, 100f, GROUND_WIDTH, GROUND_HEIGHT, RoadCenterTexture));

            roadBounds.Add(ShapeCreator(100f, GROUND_HEIGHT, GROUND_WIDTH, 0f, RoadVerticalTexture));

            roadBounds.Add(ShapeCreator(100f, GROUND_HEIGHT, GROUND_WIDTH, GROUND_HEIGHT, RoadVerticalTexture));

            roadBounds.Add(ShapeCreator(GROUND_WIDTH, 100f, 0f, GROUND_HEIGHT, RoadHorizontalTexture));

            roadBounds.Add(ShapeCreator(GROUND_WIDTH, 100f, GROUND_WIDTH + 100f, GROUND_HEIGHT, RoadHorizontalTexture));
        }

        private RectangleShape ShapeCreator(float sizeX, float sizeY, float locationX, float locationY, Texture texture)
        {
            RectangleShape rectangleShape = new RectangleShape(new Vector2f(sizeX, sizeY));
            rectangleShape.Position = new Vector2f(locationX, locationY);
            rectangleShape.Texture = texture;

            return rectangleShape;
        }
    }
}
