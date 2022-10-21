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

        private Car GetInfos()
        {
            Parent.LookAtRoadLights(); // To Refactorize !!!!!!!!!!!!!!!!!!!!!!!!!
            return AnalyseCarsSeen(Parent.LookAtCars());
        }

        private Car AnalyseCarsSeen(List<Car> cars)
        {
            if (cars.Count > 0) TrashNotDangerousCars(cars);

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
            double distanceTemp = GetDistance(cars[0]);
            Car carTemp = cars[0];

            foreach(Car car in cars)
            {
                if (distanceTemp > GetDistance(car))
                {
                    distanceTemp = GetDistance(car);
                    carTemp = car;
                }
            }
            return carTemp;
        }

        private double GetDistance(Car car) { return Math.Sqrt(Math.Pow(car.Shape.Position.Y - Parent.Shape.Position.Y, 2) + Math.Pow(car.Shape.Position.Y - Parent.Shape.Position.Y, 2)); }

        private void ChooseAnAction(Car target)
        {
            AffectStatus(target);

            switch (Parent.status)
            {
                case CarState.Go:

                    if (Parent.Engine.UpgradeTest()) Parent.UpgradeCore();
                    Parent.MoveForward(AccuracyPourcentageStackValue / 5);
                    break;

                case CarState.Decelerate:

                    if (Parent.Engine.DowngradeTest()) Parent.DowngradeCore();
                    Parent.SlowDown(AnalyseClosestCar(target));
                    break;
            }
        }

        private void AffectStatus(Car target)
        {
            if (target == null)
            {
                Parent.status = CarState.Go;
            }
            else
            {
                Parent.status = CarState.Decelerate;
            }
        }

        private float AnalyseClosestCar(Car car)
        {
            double distance = GetDistance(car);
            double targetSpeed = car.Engine.RotationSpeed;
            double targetState = 0;

            switch (car.status)
            {
                case CarState.Stop:
                    targetState = 0;
                    break;
                case CarState.Go:
                    targetState = 2;
                    break;
                case CarState.Decelerate:
                    targetState = 1;
                    break;
            }
            return (float)((targetSpeed * AccuracyPourcentageStackValue) + 
                           ((1 / distance) * AccuracyPourcentageStackValue) + 
                           (targetState * AccuracyPourcentageStackValue));
        }
    }
}
