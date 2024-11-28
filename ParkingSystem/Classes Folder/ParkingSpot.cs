using System;

namespace ParkingSystem.Classes_Folder
{
    public class ParkingSpot
    {
        public int SpotNumber { get; }
        public double Capacity { get; private set; } = 1.0; // Default for one car spot
        public bool IsOccupied => OccupiedSize >= Capacity;
        private double OccupiedSize = 0;
        private DateTime? ParkedTime = null;
        public List<Vehicle> ParkedVehicles { get; set; }


        public ParkingSpot(int spotNumber)
        {
            SpotNumber = spotNumber;
            Capacity = 1; //TODO: skall komma från konfigfil
            //ParkedVehicles = new List<Vehicle>();
        }

        public void ClearSpot()
        {
            if (ParkedTime.HasValue)
            {
                TimeSpan parkedDuration = DateTime.Now - ParkedTime.Value;
                if (parkedDuration.TotalMinutes <= 10)
                {
                    Console.WriteLine("Parking is free for the first 10 minutes.");
                }
                else
                {
                    Console.WriteLine($"Parking duration: {parkedDuration.TotalMinutes} minutes. Charges may apply.");
                }
            }
            OccupiedSize = 0;
            ParkedTime = null;
        }

        //private double GetVehicleSize(VehicleType type)
        //{
        //    return type switch
        //    {
        //        VehicleType.Car => 1.0,
        //        VehicleType.Motorcycle => 0.5,
        //        VehicleType.Bus => 2.0,
        //        VehicleType.Bicycle => 0.2,
        //        VehicleType.Helicopter => 5.0,
        //        _ => 1.0
        //    };
        //}
    }
}