using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingSystem.Classes_Folder
{
        public class Config
        {
            public required ParkingConfig ParkingConfig { get; set; }
            public required List<PriceList> PriceList { get; set; }
        }

        public class ParkingConfig
        {
            public int TotalSpots { get; set; }
            public List<string> VehicleTypes { get; set; }
            public Dictionary<string, int> VehiclesPerSpot { get; set; }

            public void IncreaseTotalSpots(int additionalSpots)
            {
                TotalSpots += additionalSpots;
            }
        }

        public class PriceList
        {
            public string VehicleType { get; set; }
            public decimal Price { get; set; }
        }
}
