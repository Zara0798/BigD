using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingSystem.Classes_Folder
{
    using ParkingSystem.Enums;
    using System;
    using System.Collections.Generic;

    public class ParkedDataConfig 
    {
        public Dictionary<string, ParkedVehicle> ParkedVehicles { get; set; }

        public ParkedDataConfig()
        {
            ParkedVehicles = new Dictionary<string, ParkedVehicle>();
        }
    }

    public class ParkedVehicle
    {
        public int Spot { get; set; }
        public DateTime StartTime { get; set; }
        public VehicleType Type { get; set; }
    }

}
