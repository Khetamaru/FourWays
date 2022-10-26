using SFML.Graphics;
using System;

namespace FourWays.Game.Objects.CarFactory.CarComponents
{
    public class Engine
    {
        private Font Arial;
        private double rotationSpeed;
        internal double RotationSpeed 
        {
            get => rotationSpeed; 
            private set
            {
                rotationSpeed = Math.Round(value, 2);
                speedTextEdit();
            } 
        }
        private Speed speed;
        internal Speed BoxSpeed 
        {
            get => speed;
            set
            {
                speed = value;
            }
        }

        public Text speedText { get; internal set; }

        public enum Speed
        {
            Back,
            One,
            Two,
            Three,
            Four,
            Five
        }

        public Engine(Speed startSpeed, Font arial)
        {
            Arial = arial;

            BoxSpeed = startSpeed; 

            RotationSpeed = BoxSpeed switch
            {
                Speed.Back => -2.0000,
                Speed.One => 0.0000,
                Speed.Two => 2.0000,
                Speed.Three => 4.0000,
                Speed.Four => 6.0000,
                Speed.Five => 8.0000,
                _ => throw new NotImplementedException()
            };
        }

        internal bool Slowdown(double moveStrength)
        {
            if (DownSpeedTest())
            {
                SlowDownRotationSpeed(moveStrength);
                if (RotationSpeed < 0.02 && RotationSpeed > 0) RotationSpeed = 0f;
                return true;
            }
            return false;
        }

        internal bool DowngradeTest()
        {
            return BoxSpeed switch
            {
                Speed.Back => (RotationSpeed >= -2 && RotationSpeed < 0),
                Speed.One => (RotationSpeed <= 0),
                Speed.Two => (RotationSpeed <= 2),
                Speed.Three => (RotationSpeed <= 4),
                Speed.Four => (RotationSpeed <= 6),
                Speed.Five => (RotationSpeed <= 8),
                _ => throw new InvalidOperationException()
            };
        }

        internal bool DownSpeedTest()
        {
            return !DowngradeTest();
        }

        private void SlowDownRotationSpeed(double moveStrength)
        {
            float limit = (float)Math.Round(1 / ((double)BoxSpeed * 1.5), 2);
            if (moveStrength > limit) moveStrength = limit;

            switch(BoxSpeed)
            {
                case Speed.One:
                    if (RotationSpeed - moveStrength <= 0) RotationSpeed = 0;
                    else RotationSpeed -= Math.Round(moveStrength, 4);
            break;
                case Speed.Two:
                    if (RotationSpeed - moveStrength <= 2) RotationSpeed = 2;
                    else RotationSpeed -= Math.Round(moveStrength, 4);
            break;
                case Speed.Three:
                    if (RotationSpeed - moveStrength <= 4) RotationSpeed = 4;
                    else RotationSpeed -= Math.Round(moveStrength, 4);
            break;
                case Speed.Four:
                    if (RotationSpeed - moveStrength <= 6) RotationSpeed = 6;
                    else RotationSpeed -= Math.Round(moveStrength, 4);
            break;
                case Speed.Five:
                    if (RotationSpeed - moveStrength <= 8) RotationSpeed = 8;
                    else RotationSpeed -= Math.Round(moveStrength, 4);
            break;
                case Speed.Back:
                    RotationSpeed -= Math.Round(moveStrength, 4);
            break;
            }
        }

        internal bool SpeedUp(double moveStrength)
        {
            if (UpSpeedTest())
            {
                SpeedUpRotationSpeed(moveStrength);
                return true;
            }
            return false;
        }

        internal bool UpgradeTest()
        {
            return BoxSpeed switch
            {
                Speed.Back => false,
                Speed.One => (RotationSpeed >= 2),
                Speed.Two => (RotationSpeed >= 4),
                Speed.Three => (RotationSpeed >= 6),
                Speed.Four => (RotationSpeed >= 8),
                Speed.Five => (RotationSpeed >= 10),
                _ => throw new InvalidOperationException()
            };
        }

        internal bool UpSpeedTest()
        {
            return !UpgradeTest();
        }

        private void SpeedUpRotationSpeed(double moveStrength)
        {
            RotationSpeed = BoxSpeed switch
            {
                Speed.One => (RotationSpeed + moveStrength >= 2) ? 2.0000 : RotationSpeed + Math.Round(moveStrength, 4),
                Speed.Two => (RotationSpeed + moveStrength >= 4) ? 4.0000 : RotationSpeed + Math.Round(moveStrength, 4),
                Speed.Three => (RotationSpeed + moveStrength >= 6) ? 6.0000 : RotationSpeed + Math.Round(moveStrength, 4),
                Speed.Four => (RotationSpeed + moveStrength >= 8) ? 8.0000 : RotationSpeed + Math.Round(moveStrength, 4),
                Speed.Five => (RotationSpeed + moveStrength >= 10) ? 10.0000 : RotationSpeed + Math.Round(moveStrength, 4),
                Speed.Back => RotationSpeed + moveStrength,
                _ => throw new NotImplementedException()
            };
        }

        internal void UpgradeCore()
        {
            if (UpgradeTest() && BoxSpeed != Speed.Five) BoxSpeed++;
        }

        internal void DowngradeCore()
        {
            if (DowngradeTest() && BoxSpeed > Speed.One) BoxSpeed--;
        }

        internal void TryPassBackSpeed()
        {
            if (DowngradeTest() && BoxSpeed >= Speed.One) BoxSpeed--;
        }

        private void speedTextEdit()
        {
            speedText = new Text(Math.Round(RotationSpeed /3600 * 50000, 1) + "Km/H", Arial, 20);
            speedText.OutlineThickness = 3;
            speedText.FillColor = Color.Blue;
            speedText.OutlineColor = Color.Cyan;
        }
    }
}