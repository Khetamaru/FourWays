using System;
using System.Collections.Generic;

namespace FourWays.Game.Objects.CarFactory.CarComponents
{
    public class Driver
    {
        Car Parent { get; }

        private const double AccuracyPourcentageStackValue = 0.2;
        private const double AccuracyPourcentageStackValueDecelerate = 0.5;

        public Driver(Car parent)
        {
            Parent = parent;
        }

        internal void Update()
        {
            GetInfos();
            ChooseAnAction();
        }

        private void GetInfos()
        {
            Parent.LookAtRoadLights();
            AnalyseCarsSeen(Parent.LookAtCars());
        }

        private void AnalyseCarsSeen(List<Car> cars)
        {
            TrashNotDangerousCars(cars);
            Car car = GetClosestCar();

            // Do Something Intelligent
        }

        private void TrashNotDangerousCars(List<Car> cars)
        {
            List<Car> trashList = FeedTrashList(cars);
            foreach (Car car in trashList)
            {
                cars.Remove(car);
            }
        }

        private void ChooseAnAction()
        {
            switch (Parent.status)
            {
                case CarState.Go:

                    if (Parent.Engine.UpgradeTest()) Parent.UpgradeCore();
                    Parent.MoveForward(AccuracyPourcentageStackValue);
                    break;

                case CarState.Decelerate:

                    if (Parent.Engine.DowngradeTest()) Parent.DowngradeCore();
                    Parent.SlowDown(AccuracyPourcentageStackValueDecelerate);
                    break;
            }
        }
    }
}
