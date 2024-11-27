using ParkingSystem.Classes_Folder;
using ParkingSystem.Enums;
using System;

namespace ParkingSystem
{
    public static class ParkingRateCalculator
    {
        public static int Calculate(VehicleType type, TimeSpan duration)
        {
            int rate = type switch
            {
                VehicleType.Car => 20,
                VehicleType.Motorcycle => 10,
                VehicleType.Bus => 50,
                VehicleType.Bicycle => 5,
                VehicleType.Helicopter => 2000,
                _ => 20
            };
            return (int)(rate * duration.TotalHours);
        }
    }
}