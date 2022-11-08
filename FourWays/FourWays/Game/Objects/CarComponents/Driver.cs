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
            Car closestCar = AnalyseCarsSeen(Parent.LookCars(true));
            Car closestCarBack = AnalyseCarsSeenBack(Parent.LookCars(false));

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

            foreach(Car car in cars)
            {
                if (Parent.direction == Direction.left || Parent.direction == Direction.right)
                {
                    switch (Math.Abs((Parent.move + car.move).X))
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
                        case 2: if (Parent.Engine.RotationSpeed <= car.Engine.RotationSpeed && GetDistance(car.Shape) > Parent.SecurityDistance) trashList.Add(car); break;
                    }
                }
                else
                {
                    switch (Math.Abs((Parent.move + car.move).Y))
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
                        case 2: if (Parent.Engine.RotationSpeed <= car.Engine.RotationSpeed && GetDistance(car.Shape) > Parent.SecurityDistance) trashList.Add(car); break;
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

        internal double GetDistance(RectangleShape shape)
        {
            float X1 = shape.Position.X + (shape.Size.X / 2);
            float Y1 = shape.Position.Y + (shape.Size.Y / 2);

            float X2 = Parent.Shape.Position.X + (Parent.Shape.Size.X / 2);
            float Y2 = Parent.Shape.Position.Y + (Parent.Shape.Size.Y / 2);


            return DistanceCalcul(X1, X2, Y1, Y2)
                 - DistanceCalcul(X1, shape.Position.X, Y1, shape.Position.Y)
                 - DistanceCalcul(X2, Parent.Shape.Position.X, Y2, Parent.Shape.Position.Y);
        }

        private double DistanceCalcul(float X1, float X2, float Y1, float Y2) => Math.Abs(Math.Sqrt(Math.Pow(Y2 - Y1, 2) + Math.Pow(X2 - X1, 2)));

        private void ChooseAnAction((GameObject, Car) target)
        {
            AffectStatus(target.Item1);

            switch (Parent.status)
            {
                case CarState.Go:

                    if (Parent.Engine.UpgradeTest() && Parent.Engine.BoxSpeed != Engine.Speed.Five) Parent.UpgradeCore();
                    else Parent.MoveForward();
                    
                    break;

                case CarState.Decelerate:

                    if (Parent.Engine.DowngradeTest()) Parent.DowngradeCore();
                    if (target.Item1 as Car != null) Parent.SlowDown(GetSlowStrengthCar(target.Item1 as Car));
                    else if (target.Item1 as RoadLight != null) Parent.SlowDown(GetSlowStrengthStopLine(target.Item1 as RoadLight));
                    else { throw new ArgumentNullException(); }
                    break;

                case CarState.BackForward:

                    if (target.Item2 != null) Parent.MoveBack(AnalyseClosestCarBack(target.Item2));
                    else Parent.MoveBack(AccuracyPourcentageStackValue);
                    break;
            }
        }

        private static double SpeedBoost(double speed) => speed * 2;

        private double GetSlowStrengthCar(Car car) => GetSlowStrength(Math.Abs(GetDistance(car.Shape)));

        private double GetSlowStrengthStopLine(RoadLight roadLight) => GetSlowStrength(Math.Abs(GetDistance(roadLight.StopLine)));

        private double GetSlowStrength(double distance)
        {
            float divider = 5;
            double speed = Parent.Engine.RotationSpeed / 3600 * 50000;
            double slowDownDistance = Math.Round(speed / 10, 0) * Math.Round(speed / 10, 0) * divider;

            if (distance < slowDownDistance)
            {
                return Math.Abs(speed - Math.Round(Math.Sqrt(distance / divider) * 10, 0));
            }
            return 0;
        }

        private void AffectStatus(GameObject target)
        {
            if (target == null)
            {
                Parent.status = CarState.Go;
            }
            else if ((target as Car) != null && GetDistance((target as Car).Shape) < Parent.SecurityDistance)
            {
                Parent.status = CarState.BackForward;
            }
            else
            {
                Parent.status = CarState.Decelerate;
            }
        }

        private double AnalyseClosestCarBack(Car target)
        {
            double distance = GetDistance(target.Shape);
            double targetSpeed = 1 / target.Engine.RotationSpeed;

            return (float)Math.Abs((targetSpeed * AccuracyPourcentageStackValue) +
                                   (distance * AccuracyPourcentageStackValue));
        }
    }
}
