using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace FourWays.Game.Objects.CarFactory.CarComponents
{
    public class Driver
    {
        Car Parent { get; }

        private const double AccuracyPourcentageStackValue = 0.1;

        public Driver(Car parent)
        {
            Parent = parent;
        }

        internal void Update()
        {
            ChooseAnAction(GetInfos());
        }

        private (GameObject, Car) GetInfos()
        {
            RoadLight closestRoadLight = Parent.LookAtRoadLights();
            Car closestCar = AnalyseCarsSeen(Parent.LookForward());
            Car closestCarBack = AnalyseCarsSeenBack(Parent.LookBack());

            return (ClosestObstacle(closestRoadLight, closestCar), closestCarBack);
        }

        private GameObject ClosestObstacle(RoadLight closestRoadLight, Car closestCar)
        {
            if (closestRoadLight == null) return closestCar;
            else if (closestCar == null) return closestRoadLight;

            return GetDistance(closestCar.Shape) < GetDistance(closestRoadLight.StopLine) ? closestCar : closestRoadLight;
        }

        private Car AnalyseCarsSeen(List<Car> cars)
        {
            if (cars.Count > 0) TrashNotDangerousCars(cars);

            return cars.Count > 0 ? GetClosestCar(cars) : null;
        }

        private Car AnalyseCarsSeenBack(List<Car> cars)
        {
            return cars.Count > 0 ? GetClosestCar(cars) : null;
        }

        private void TrashNotDangerousCars(List<Car> cars)
        {
            List<Car> trashList = FeedTrashList(cars);
            foreach (Car car in trashList)
            {
                cars.Remove(car);
            }
        }

        private List<Car> FeedTrashList(List<Car> cars)
        {
            List<Car> trashList = new List<Car>();
            Vector2f orientationValue;

            foreach(Car car in cars)
            {
                orientationValue = Parent.move + car.move;
                if (Parent.direction == Direction.left || Parent.direction == Direction.right)
                {
                    switch (Math.Abs(orientationValue.X))
                    {
                        case 0: trashList.Add(car); break;
                        case 1:
                            if (car.RoadLight.state == RoadLightState.Red && car.IsBehindTheLine()) trashList.Add(car);
                            else if (car.direction == Direction.down)
                            {
                                if (car.Shape.Position.Y > (Parent.Shape.Position.Y + Parent.Shape.Size.Y)) trashList.Add(car);
                            }
                            else if (car.Shape.Position.Y + car.Shape.Size.Y < Parent.Shape.Position.Y) trashList.Add(car);
                            break;
                        case 2: if (Parent.Engine.RotationSpeed <= car.Engine.RotationSpeed) trashList.Add(car); break;
                    }
                }
                else
                {
                    switch (Math.Abs(orientationValue.Y))
                    {
                        case 0: trashList.Add(car); break;
                        case 1:
                            if (car.RoadLight.state == RoadLightState.Red && car.IsBehindTheLine()) trashList.Add(car);
                            if (car.direction == Direction.right)
                            {
                                if (car.Shape.Position.X > (Parent.Shape.Position.X + Parent.Shape.Size.X)) trashList.Add(car);
                            }
                            else if (car.Shape.Position.X + car.Shape.Size.X < Parent.Shape.Position.X) trashList.Add(car);
                            break;
                        case 2: if (Parent.Engine.RotationSpeed <= car.Engine.RotationSpeed) trashList.Add(car); break;
                    }
                }
            }
            return trashList;
        }

        private Car GetClosestCar(List<Car> cars)
        {
            double distanceTemp = GetDistance(cars[0].Shape);
            Car carTemp = cars[0];

            foreach(Car car in cars)
            {
                if (distanceTemp > GetDistance(car.Shape))
                {
                    distanceTemp = GetDistance(car.Shape);
                    carTemp = car;
                }
            }
            return carTemp;
        }

        private double GetDistance(RectangleShape shape)
        {
            RectangleShape targetRealShape = new RectangleShape();
            targetRealShape.Position.X = shape.Position.X + (shape.Size.X / 2)
            RectangleShape realShape = new RectangleShape();
            return Parent.direction switch
            {
                Direction.up => shape.Position.Y - Parent.Shape.Position.Y,
                Direction.down => shape.Position.Y - Parent.Shape.Position.Y + Parent.Shape.Size.Y,
                Direction.left => shape.Position.X - Parent.Shape.Position.X,
                Direction.right => shape.Position.X - Parent.Shape.Position.X + Parent.Shape.Size.X,
                _ => throw new NotImplementedException()
            };
        }

        private void ChooseAnAction((GameObject, Car) target)
        {
            AffectStatus(target.Item1);

            switch (Parent.status)
            {
                case CarState.Go:

                    if (Parent.Engine.UpgradeTest()) Parent.UpgradeCore();
                    if (Parent.Engine.BoxSpeed == Engine.Speed.One) Parent.MoveForward(SpeedBoost(AccuracyPourcentageStackValue / 5));
                    else Parent.MoveForward(AccuracyPourcentageStackValue / 5);
                    
                    break;

                case CarState.Decelerate:

                    if (target.Item1 as Car != null)
                    {

                        if (Parent.Engine.DowngradeTest()) Parent.DowngradeCore();
                        Parent.SlowDown(AnalyseClosestCar(target.Item1 as Car));
                    }
                    else if (target.Item1 as RoadLight != null)
                    {
                        if (Parent.Engine.DowngradeTest()) Parent.DowngradeCore();
                        Parent.SlowDown(GetSlowStrengthStopLine(target.Item1 as RoadLight));
                    }
                    else { throw new ArgumentNullException(); }
                    break;

                case CarState.BackForward:

                    List<Car> list = Parent.LookBack();
                    if (list.Count > 0) Parent.MoveBack(AnalyseClosestCarBack(target.Item2));
                    else
                    {
                        if (target.Item1 as Car != null)
                        {

                            if (Parent.Engine.DowngradeTest()) Parent.DowngradeCore();
                            Parent.SlowDown(AnalyseClosestCar(target.Item1 as Car));
                        }
                        else if (target.Item1 as RoadLight != null)
                        {
                            if (Parent.Engine.DowngradeTest()) Parent.DowngradeCore();
                            Parent.SlowDown(GetSlowStrengthStopLine(target.Item1 as RoadLight));
                        }
                        else { throw new ArgumentNullException(); }
                    }
                    break;
            }
        }

        private static double SpeedBoost(double speed) => speed * 2;

        private double GetSlowStrengthStopLine(RoadLight roadLight)
        {
            double distance = Math.Abs(GetDistance(roadLight.StopLine));
            double speed = Parent.Engine.RotationSpeed / 3600 * 50000;
            double slowDownDistance = Math.Round(speed / 10, 0) * Math.Round(speed / 10, 0);

            if (distance < slowDownDistance)
            {
                return speed - Math.Round(Math.Sqrt(distance) * 10, 0);
            }
            return 0;
        }

        private void AffectStatus(GameObject target)
        {
            if (target == null)
            {
                Parent.status = CarState.Go;
            }
            else if ((target as Car) != null && GetDistance((target as Car).Shape) < Car.SecurityDistance)
            {
                Parent.status = CarState.BackForward;
            }
            else
            {
                Parent.status = CarState.Decelerate;
            }
        }

        private float AnalyseClosestCar(Car target)
        {
            double distance = 1 / GetDistance(target.Shape);
            double targetSpeed = target.Engine.RotationSpeed;
            double targetState = 0;

            switch (target.status)
            {
                case CarState.Go:
                    targetState = 2;
                    break;
                case CarState.Decelerate:
                    targetState = 1;
                    break;
            }
            return (float)((targetSpeed * AccuracyPourcentageStackValue) + 
                           (distance * AccuracyPourcentageStackValue) + 
                           (targetState * AccuracyPourcentageStackValue));
        }

        private double AnalyseClosestCarBack(Car target)
        {
            double distance = GetDistance(target.Shape);
            double targetSpeed = 1 / target.Engine.RotationSpeed;

            return (float)((targetSpeed * AccuracyPourcentageStackValue) +
                           (distance * AccuracyPourcentageStackValue));
        }
    }
}
