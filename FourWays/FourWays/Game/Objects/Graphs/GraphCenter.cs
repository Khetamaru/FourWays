using FourWays.Game.Objects.ObjectFactory;
using FourWays.Loop;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using static FourWays.Game.Objects.Graphs.DeathGraph;

namespace FourWays.Game.Objects.Graphs
{
    public class GraphCenter
    {
        internal GameLoop Parent;
        internal DeathGraph DeathGraph;
        internal SucessGraph SucessGraph;
        internal WayGraph WayGraph;
        internal Legend Legend;
        internal ChocDetailGraph ChocDetailGraph;

        public GraphCenter(GameLoop parent, Vector2f position, Color fontColor, List<CarColor> carColors)
        {
            Parent = parent;

            DeathGraph = new DeathGraph(Parent, position, fontColor, carColors);
            SucessGraph = new SucessGraph(Parent, new Vector2f(position.X - DeathGraph.Size.X, position.Y), fontColor);
            WayGraph = new WayGraph(Parent, new Vector2f(position.X - DeathGraph.Size.X, position.Y + SucessGraph.Size.Y + 5f), fontColor);
            Legend = new Legend(Parent, new Vector2f(15f, 90f), fontColor);
            ChocDetailGraph = new ChocDetailGraph(Parent, new Vector2f(Parent.Window.Size.X / 2 + 115f, Parent.Window.Size.Y / 2 + 65f), fontColor);
        }

        internal void LoadContent()
        {
            DebugUtility.LoadContent();
        }

        internal void DrawGraphs()
        {
            DebugUtility.DrawPerformanceData(Parent, Color.White);
            DeathGraph.DrawDataTab();
            SucessGraph.DrawDataTab();
            WayGraph.DrawDataTab();

            Legend.DrawDataTab();

            ChocDetailGraph.DrawDataTab();
        }

        internal void DeathIncrement(CarColor deathColor, CarColor deathColor2)
        {
            DeathGraph.AddDeathCounter(deathColor);
            SucessGraph.AddResultCounter(false);
            ChocDetailGraph.AddKeyValuePair(new KeyValuePair<CarColor, CarColor>(deathColor, deathColor2));
        }

        internal void SucessIncrement(Car car)
        {
            SucessGraph.AddResultCounter(true);
            if (car.direction == car.Objective.Direction) WayGraph.AddResultCounter(true);
            else WayGraph.AddResultCounter(false);
        }
    }
}
