using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourWays.Game.Objects
{
    public class VehiculeCore
    {
        float rotationSpeed { get; set; }
        public Speed speed { get; set; }

        public enum Speed
        {
            Back,
            One,
            Two,
            Three,
            Four,
            Five
        }

        public VehiculeCore(float startRotationSpeed, Speed startSpeed)
        {
            rotationSpeed = startRotationSpeed;
            speed = startSpeed;
        }
    }
}
