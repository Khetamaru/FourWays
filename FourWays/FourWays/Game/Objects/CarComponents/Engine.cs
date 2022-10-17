using System;

namespace FourWays.Game.Objects.CarFactory.CarComponents
{
    public class Engine
    {
        internal double RotationSpeed { get; private set; }
        private Speed speed;

        public enum Speed
        {
            Back,
            One,
            Two,
            Three,
            Four,
            Five
        }

        public Engine(float startRotationSpeed, Speed startSpeed)
        {
            RotationSpeed = startRotationSpeed;
            speed = startSpeed;
        }

        internal bool Slowdown(float moveStrength)
        {
            if (SpeedLimitTestDown())
            {
                SlowDownRotationSpeed(moveStrength);
                return true;
            }
            return false;
        }

        private bool SpeedLimitTestDown()
        {
            return speed switch
            {
                Speed.Back => false,
                Speed.One => !(RotationSpeed < 0),
                Speed.Two => !(RotationSpeed < 0.2),
                Speed.Three => !(RotationSpeed < 0.4),
                Speed.Four => !(RotationSpeed < 0.6),
                Speed.Five => !(RotationSpeed < 0.8),
                _ => throw new InvalidOperationException()
            };
        }

        private void SlowDownRotationSpeed(double moveStrength)
        {
            RotationSpeed = speed switch
            {
                Speed.One => !(RotationSpeed - moveStrength < 0) ? 0 : RotationSpeed - moveStrength,
                Speed.Two => !(RotationSpeed - moveStrength < 0.2) ? 0.2 : RotationSpeed - moveStrength,
                Speed.Three => !(RotationSpeed - moveStrength < 0.4) ? 0.4 : RotationSpeed - moveStrength,
                Speed.Four => !(RotationSpeed - moveStrength < 0.6) ? 0.6 : RotationSpeed - moveStrength,
                Speed.Five => !(RotationSpeed - moveStrength < 0.8) ? 0.8 : RotationSpeed - moveStrength,
                _ => throw new InvalidOperationException()
            };
        }

        internal bool SpeedUp(float moveStrength)
        {
            if (SpeedLimitTestUp())
            {
                SpeedUpRotationSpeed(moveStrength);
                return true;
            }
            return false;
        }

        private bool SpeedLimitTestUp()
        {
            return speed switch
            {
                Speed.Back => false,
                Speed.One => !(RotationSpeed > 0.2),
                Speed.Two => !(RotationSpeed > 0.4),
                Speed.Three => !(RotationSpeed > 0.6),
                Speed.Four => !(RotationSpeed > 0.8),
                Speed.Five => !(RotationSpeed > 1),
                _ => throw new InvalidOperationException()
            };
        }

        private void SpeedUpRotationSpeed(double moveStrength)
        {
            RotationSpeed = speed switch
            {
                Speed.One => !(RotationSpeed - moveStrength > 0.2) ? 0.2 : RotationSpeed + moveStrength,
                Speed.Two => !(RotationSpeed - moveStrength > 0.4) ? 0.4 : RotationSpeed + moveStrength,
                Speed.Three => !(RotationSpeed - moveStrength > 0.6) ? 0.6 : RotationSpeed + moveStrength,
                Speed.Four => !(RotationSpeed - moveStrength > 0.8) ? 0.8 : RotationSpeed + moveStrength,
                Speed.Five => !(RotationSpeed - moveStrength > 1) ? 1 : RotationSpeed + moveStrength,
                _ => throw new InvalidOperationException()
            };
        }

        internal void UpgradeCore()
        {
            if (!SpeedLimitTestUp() && speed != Speed.Five) speed++;
        }

        internal void DowngradeCore()
        {
            if (!SpeedLimitTestDown() && speed != Speed.Back) speed--;
        }
    }
}