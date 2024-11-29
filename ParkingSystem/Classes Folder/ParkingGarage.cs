using System;
using ParkingSystem.Classes_Folder;
using System.Text.Json;
using System.IO;
using ParkingSystem.Enums;
using System.Text;

namespace ParkingSystem.Classes_Folder
{

    public class ParkingGarage
    {
        public Config config;
        public ParkedDataConfig parkedDataConfig;
        public List<ParkingSpot> Garage { get; set; }


        public ParkingGarage(Config parkingConfig, ParkedDataConfig parkedDataConfig)
        {
            this.config = parkingConfig ?? throw new ArgumentNullException(nameof(parkingConfig));
            this.parkedDataConfig = parkedDataConfig ?? throw new ArgumentNullException(nameof(parkedDataConfig));
            InitializeGarage();
        }

        public ParkingGarage(int totalSpots)
        {
            Garage = new List<ParkingSpot>(totalSpots);
            //parkedVehicles = new Dictionary<string, (int spot, DateTime startTime, VehicleType type)>();

            for (int i = 0; i < totalSpots; i++)
            {
                Garage.Add(new ParkingSpot(i + 1));
            }
        }

        private void InitializeGarage()
        {
            Garage = new List<ParkingSpot>(config.ParkingConfig.TotalSpots);
            for (int i = 0; i < config.ParkingConfig.TotalSpots; i++)
            {
                Garage.Add(new ParkingSpot(i + 1));
            }

            foreach (var kvp in parkedDataConfig.ParkedVehicles)
            {
                string licensePlate = kvp.Key;
                ParkedVehicle parkedVehicle = kvp.Value;

                // Find the corresponding parking spot
                ParkingSpot spot = Garage.FirstOrDefault(s => s.SpotNumber == parkedVehicle.Spot);
                if (spot != null)
                {
                    spot.Capacity = spot.GetVehicleSize(parkedVehicle.Type);
                    // Create a specific type of vehicle based on VehicleType
                    Vehicle vehicle = CreateVehicle(licensePlate, parkedVehicle.Type);
                    vehicle.PricePerHour = GetVehiclePricePerHour(parkedVehicle.Type);
                    vehicle.StartTime = parkedVehicle.StartTime; // Assign StartTime from parkedVehicle
                    vehicle.EndTime = parkedVehicle.EndTime; // Assign EndTime from parkedVehicle if available

                    spot.ParkedVehicles.Add(vehicle);
                    spot.OccupiedSize = spot.GetVehicleSize(parkedVehicle.Type);
 
                }
            }
        }

        public Vehicle CreateVehicle(string licensePlate, VehicleType type)
        {
            return type switch
            {
                VehicleType.Car => new Car(licensePlate),
                VehicleType.Motorcycle => new Motorcycle(licensePlate),
                VehicleType.Bus => new Bus(licensePlate),
                VehicleType.Helicopter => new Helicopter(licensePlate),
                _ => throw new ArgumentException("Invalid vehicle type", nameof(type)),
            };
        }

        public int GetVehiclePricePerHour(VehicleType type)
        {
            return type switch
            {
                VehicleType.Car => 10,
                VehicleType.Motorcycle => 5,
                VehicleType.Bus => 20,
                VehicleType.Bicycle => 2,
                _ => throw new ArgumentOutOfRangeException(nameof(type), $"Not expected vehicle type value: {type}")
            };
        }
        public void SaveParkingDataToFile(string filePath)
        {
            throw new NotImplementedException();
        }


        public class ParkedVehicleInfo
        {
            public int SpotNumber { get; set; }
            public string Status { get; set; }
            public string LicensePlate { get; set; }
            public double? ParkingTime { get; set; }
            public int CurrentFee { get; set; }
        }

        public List<ParkedVehicleInfo> GetParkedVehiclesInfo()
        {
            var parkedVehiclesInfo = new List<ParkedVehicleInfo>();

            foreach (var spot in Garage)
            {
                if (spot.ParkedVehicles != null && spot.ParkedVehicles.Count > 0)
                {
                    foreach (var vehicle in spot.ParkedVehicles)
                    {
                        var info = new ParkedVehicleInfo
                        {
                            SpotNumber = spot.SpotNumber,
                            Status = spot.IsOccupied ? "Occupied" : "Empty",
                            LicensePlate = vehicle.LicensePlate,
                            ParkingTime = (CalculateParkingTime(vehicle.StartTime, vehicle.EndTime).DurationInMins),
                            CurrentFee = CalculateParkingFee(CalculateParkingTime(vehicle.StartTime, vehicle.EndTime).DurationInMins, vehicle)
                        };
                        parkedVehiclesInfo.Add(info);
                    }
                }
            }

            return parkedVehiclesInfo;
        }


        public class ParkingTimeInfo
        {
            public double DurationInMins { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }

        static ParkingTimeInfo CalculateParkingTime(DateTime startTime, DateTime? endTime = null)
        {
            DateTime effectiveEndTime = endTime ?? DateTime.Now;
            TimeSpan duration = effectiveEndTime - startTime;
            return new ParkingTimeInfo
            {
                DurationInMins = duration.TotalMinutes, // Change to minutes using TotalHours
                StartTime = startTime,
                EndTime = effectiveEndTime
            };
        }

        public void ParkVehicle(ParkingGarage garage, Vehicle vehicle, int spotNumber)
        {
            // Find the corresponding parking spot
            ParkingSpot spot = garage.Garage.FirstOrDefault(s => s.SpotNumber == spotNumber);
            if (spot != null && !spot.IsOccupied)
            {
                vehicle.StartTime = DateTime.Now; // Set the current time as StartTime
                spot.ParkedVehicles.Add(vehicle);
                spot.OccupiedSize = vehicle.Size;
                spot.ParkedTime = DateTime.Now; // Set the current time as ParkedTime
            }
            else
            {
                Console.WriteLine("The spot is either occupied or does not exist.");
            }
        }


        public class ParkingSpotInfo
        {
            public int SpotNumber { get; set; }
            public string Status { get; set; }
            public string LicensePlate { get; set; }
        }

        public List<ParkingSpotInfo> GetParkingMapInfo()
        {
            var parkingMapInfo = new List<ParkingSpotInfo>();

            foreach (var spot in Garage)
            {
                if (spot == null) continue; // Add this line to skip null spots

                var info = new ParkingSpotInfo
                {
                    SpotNumber = spot.SpotNumber,
                    Status = spot.IsOccupied ? "Occupied" : "Empty",
                    LicensePlate = spot.ParkedVehicles.Count > 0 ? spot.ParkedVehicles[0].LicensePlate : "-"
                };
                parkingMapInfo.Add(info);
            }

            return parkingMapInfo;
        }


        public bool MoveVehicle(string licensePlate, int newSpotNumber)
        {
            foreach (var spot in Garage)
            {
                var vehicle = spot.ParkedVehicles.FirstOrDefault(v => v.LicensePlate == licensePlate);
                if (vehicle != null)
                {
                    // Find the new spot
                    var newSpot = Garage.FirstOrDefault(s => s.SpotNumber == newSpotNumber);
                    if (newSpot != null && !newSpot.IsOccupied)
                    {
                        // Move the vehicle to the new spot
                        spot.ParkedVehicles.Remove(vehicle);
                        spot.OccupiedSize = 0;

                        newSpot.ParkedVehicles.Add(vehicle);
                        newSpot.OccupiedSize = vehicle.Size;

                        return true;
                    }
                    else
                    {
                        Console.WriteLine("The new spot is either occupied or does not exist.");
                        return false;
                    }
                }
            }
            return false; // Vehicle not found
        }

        private int CalculateParkingFee(double? parkedTime, Vehicle vehicle)
        {
            if (!parkedTime.HasValue)
            {
                return 0;
            }

            // Subtract the first 10 minutes (10 minutes) as free time
            double chargeableMinutes = Math.Max(0, parkedTime.Value - 10);

            // Calculate the fee based on the chargeable minutes and the vehicle's price per hour
            // Convert price per hour to price per minute
            double pricePerMinute = vehicle.PricePerHour / 60.0;
            return (int)(chargeableMinutes * pricePerMinute);
        }


        public ParkedVehicleInfo? SearchVehicleByLicensePlate(string licensePlate)
        {
            foreach (var spot in Garage)
            {
                var vehicle = spot.ParkedVehicles.FirstOrDefault(v => v.LicensePlate == licensePlate);
                if (vehicle != null)
                {
                    return new ParkedVehicleInfo
                    {
                        SpotNumber = spot.SpotNumber,
                        Status = spot.IsOccupied ? "Occupied" : "Empty",
                        LicensePlate = vehicle.LicensePlate,
                        ParkingTime = CalculateParkingTime(vehicle.StartTime, vehicle.EndTime).DurationInMins,
                        CurrentFee = CalculateParkingFee(CalculateParkingTime(vehicle.StartTime, vehicle.EndTime).DurationInMins, vehicle)
                    };
                }
            }
            return null; // Vehicle not found
        }


        public (bool success, ParkedVehicleInfo? vehicleInfo) RetrieveVehicle(string licensePlate)
        {
            foreach (var spot in Garage)
            {
                var vehicle = spot.ParkedVehicles.FirstOrDefault(v => v.LicensePlate == licensePlate);
                if (vehicle != null)
                {
                    var parkingTimeInfo = CalculateParkingTime(vehicle.StartTime, DateTime.Now);
                    var fee = CalculateParkingFee(parkingTimeInfo.DurationInMins, vehicle);

                    var vehicleInfo = new ParkedVehicleInfo
                    {
                        SpotNumber = spot.SpotNumber,
                        Status = "Retrieved",
                        LicensePlate = vehicle.LicensePlate,
                        ParkingTime = parkingTimeInfo.DurationInMins,
                        CurrentFee = fee
                    };

                    spot.ParkedVehicles.Remove(vehicle);
                    spot.OccupiedSize = 0;

                    return (true, vehicleInfo);
                }
            }
            return (false, null); // Vehicle not found
        }

        public void ClearParkingGarage()
        {
            foreach (var spot in Garage)
            {
                spot.ParkedVehicles.Clear();
                spot.OccupiedSize = 0;
            }
        }


    }
}
