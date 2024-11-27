using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingSystem
{
    public class ConfigData
    {
        // Configuration properties
        public int TotalSpots { get; set; }
        public int VIPSpots { get; set; }

        // Method to increase TotalSpots
        public void IncreaseTotalSpots(int additionalSpots)
        {
            if (additionalSpots > 0)
            {
                TotalSpots += additionalSpots;
                Console.WriteLine($"TotalSpots increased by {additionalSpots}. New TotalSpots: {TotalSpots}");
            }
            else
            {
                Console.WriteLine("Additional spots must be greater than zero.");
            }
        }
    }
}
