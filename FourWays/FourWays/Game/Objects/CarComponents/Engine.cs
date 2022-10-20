using SFML.Graphics;
using SFML.System;
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
            } 
        }
        private Speed speed;
        internal Speed BoxSpeed 
        {
            get => speed;
            set
            {
                speed = value;
                speedTextEdit();
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
                Speed.Back => -2,
                Speed.One => 0,
                Speed.Two => 2,
                Speed.Three => 4,
                Speed.Four => 6,
                Speed.Five => 8,
                _ => throw new NotImplementedException()
            };
        }

        internal bool Slowdown(double moveStrength)
        {
            if (DownSpeedTest())
            {
                SlowDownRotationSpeed(moveStrength);
                return true;
            }
            return false;
        }

        internal bool DowngradeTest()
        {
            return BoxSpeed switch
            {
                Speed.Back => false,
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
            RotationSpeed = BoxSpeed switch
            {
                Speed.One => (RotationSpeed - moveStrength <= 0) ? 0.0000 : RotationSpeed - moveStrength,
                Speed.Two => (RotationSpeed - moveStrength <= 2) ? 2.0000 : RotationSpeed - moveStrength,
                Speed.Three => (RotationSpeed - moveStrength <= 4) ? 4.0000 : RotationSpeed - moveStrength,
                Speed.Four => (RotationSpeed - moveStrength <= 6) ? 6.0000 : RotationSpeed - moveStrength,
                Speed.Five => (RotationSpeed - moveStrength <= 8) ? 8.0000 : RotationSpeed - moveStrength,
                Speed.Back => RotationSpeed - moveStrength
            };
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
                Speed.One => (RotationSpeed + moveStrength >= 2) ? 2.0000 : RotationSpeed + moveStrength,
                Speed.Two => (RotationSpeed + moveStrength >= 4) ? 4.0000 : RotationSpeed + moveStrength,
                Speed.Three => (RotationSpeed + moveStrength >= 6) ? 6.0000 : RotationSpeed + moveStrength,
                Speed.Four => (RotationSpeed + moveStrength >= 8) ? 8.0000 : RotationSpeed + moveStrength,
                Speed.Five => (RotationSpeed + moveStrength >= 10) ? 10.0000 : RotationSpeed + moveStrength,
                Speed.Back => RotationSpeed + moveStrength
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

        private void speedTextEdit()
        {
            speedText = new Text(BoxSpeed.ToString(), Arial, 20);
            speedText.OutlineThickness = 3;
            speedText.FillColor = Color.Blue;
            speedText.OutlineColor = Color.Cyan;
        }
    }
}