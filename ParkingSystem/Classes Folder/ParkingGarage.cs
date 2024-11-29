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

        //private double GetVehicleSize(VehicleType type)
        //{
        //    return type switch
        //    {
        //        VehicleType.Car => 1.0,
        //        VehicleType.Motorcycle => 0.5,
        //        VehicleType.Bus => 3.0,
        //        VehicleType.Bicycle => 0.2,
        //        VehicleType.Helicopter => 5.0,
        //        _ => throw new ArgumentOutOfRangeException(nameof(type), $"Not expected vehicle type value: {type}")
        //    };f
        //}

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

        //private int CalculateParkingFee(DateTime? parkedTime, Vehicle vehicle)
        //{
        //    if (!parkedTime.HasValue)
        //    {
        //        return 0;
        //    }

        //    // Calculate the parking time using the existing method
        //    var parkingTimeInfo = CalculateParkingTime(parkedTime.Value);

        //    // Subtract the first 10 minutes (0.1667 hours) as free time
        //    double chargeableHours = Math.Max(0, parkingTimeInfo.DurationInMins - 0.1667);

        //    // Calculate the fee based on the chargeable hours and the vehicle's price per hour
        //    return (int)(chargeableHours * vehicle.PricePerHour);
        //}

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



        // Söka efter fordon (regnummer)
        //public bool RetrieveVehicle(string licensePlate)
        //{
        //    if (!parkedVehicles.TryGetValue(licensePlate, out var vehicleInfo)) return false;

        //    parkingSpots[vehicleInfo.spot].ClearSpot();
        //    parkedVehicles.Remove(licensePlate);
        //    return true;
        //}

        //public (int spot, DateTime startTime, VehicleType type)? FindVehicle(string licensePlate)
        //{
        //    if (parkedVehicles.TryGetValue(licensePlate, out var vehicleInfo))
        //    {
        //        return vehicleInfo;
        //    }
        //    return null;
        //}






        /* TODO: P-huset behöver publika metoder för:
         * Parkera ett fordon
         * Hämta ut ett fordon
         * Flytta ett fordon
         * Söka efter fordon (regnummer)
         * Visa hela husets innehåll
         * 
         * Vi behöver även några privata hjälpmetoder:
         * Hitta ledig plats för ett fordon
         * Skapa ett fordon
         * Ta fram ett fordon, givet ett regnummer (variant på sökning)
         */

        //static void ClearParkingGarage(string[] garage)
        //{
        //    Console.Write("Enter admin password to clear the parking garage: ");
        //    string? password = Console.ReadLine();

        //    if (password == AdminPassword)
        //    {
        //        Array.Clear(garage, 0, garage.Length);
        //        parkingTimes.Clear();
        //        Console.WriteLine("Parking garage has been cleared.");
        //    }
        //    else
        //    {
        //        Console.WriteLine("Incorrect password. Access denied.");
        //    }
        //}

        //static double GetRequiredSpots(string vehicleType)
        //{
        //    return vehicleType.ToLower() switch
        //    {
        //        "car" => 1,
        //        "motorcycle" => 0.5,
        //        "bus" => 3,
        //        "helicopter" => 5,
        //        "bike" => 0.2,
        //        _ => 1 // standard för 1 parkeringsplats
        //    };
        //}


        //static int FindAvailableSpots(string[] garage, double requiredSpots)
        //{
        //    int intRequiredSpots = (int)Math.Ceiling(requiredSpots);

        //    for (int i = 0; i <= garage.Length - intRequiredSpots; i++)
        //    {
        //        bool allSpotsAvailable = true;
        //        for (int j = 0; j < intRequiredSpots; j++)
        //        {
        //            if (!string.IsNullOrEmpty(garage[i + j]))
        //            {
        //                allSpotsAvailable = false;
        //                break;
        //            }
        //        }
        //        if (allSpotsAvailable)
        //        {
        //            Console.WriteLine($"Available spot found at index {i} for {intRequiredSpots} spots.");
        //            return i;
        //        }
        //    }
        //    Console.WriteLine("No available spots found.");
        //    return -1;
        //}

        //static void RetrieveVehicle(string[] garage)
        //{
        //    Console.Write("Enter license plate: ");
        //    string? licensePlate = Console.ReadLine();

        //    if (string.IsNullOrEmpty(licensePlate))
        //    {
        //        Console.WriteLine("License plate cannot be empty.");
        //        return;
        //    }

        //    // Hitta fordon
        //    int vehicleSpot = Array.FindIndex(garage, spot => spot?.Contains($"#{licensePlate}") == true);

        //    if (vehicleSpot == -1)
        //    {
        //        Console.WriteLine("Vehicle not found.");
        //        return;
        //    }

        //    // logga nuvarande parkeringstider
        //    Console.WriteLine("Current parking times:");
        //    foreach (var entry in parkingTimes)
        //    {
        //        Console.WriteLine($"License Plate: {entry.Key}, Start Time: {entry.Value}");
        //    }

        //    // räkna parkerings avgift
        //    string[] vehicleData = garage[vehicleSpot].Split('#');
        //    string vehicleType = vehicleData[0];

        //    if (parkingTimes.TryGetValue(licensePlate, out DateTime startTime))
        //    {
        //        TimeSpan duration = DateTime.Now - startTime;
        //        int fee = CalculateParkingCost(duration, vehicleType);

        //        // ta bort fordonet
        //        for (int i = 0; i < (int)Math.Ceiling(GetRequiredSpots(vehicleType)); i++)
        //        {
        //            garage[vehicleSpot + i] = string.Empty;
        //        }
        //        parkingTimes.Remove(licensePlate);

        //        Console.WriteLine($"Vehicle retrieved from spot {vehicleSpot + 1}. Parking fee: {fee} CZK.");
        //    }
        //    else
        //    {
        //        Console.WriteLine("Error: Parking time not found.");
        //    }
        //}

        //static void MoveVehicle(string[] garage)
        //{
        //    Console.Write("Enter license plate of the vehicle to move: ");
        //    string? licensePlate = Console.ReadLine();

        //    if (string.IsNullOrEmpty(licensePlate))
        //    {
        //        Console.WriteLine("License plate cannot be empty.");
        //        return;
        //    }

        //    // hitta fordonet
        //    int currentSpot = Array.FindIndex(garage, spot => spot?.Contains($"#{licensePlate}") == true);

        //    if (currentSpot == -1)
        //    {
        //        Console.WriteLine("Vehicle not found.");
        //        return;
        //    }

        //    // hitta tillgänglig plats
        //    int availableSpot = Array.FindIndex(garage, spot => string.IsNullOrEmpty(spot));

        //    if (availableSpot == -1)
        //    {
        //        Console.WriteLine("No available spots.");
        //        return;
        //    }

        //    // flytta fordonet
        //    garage[availableSpot] = garage[currentSpot];
        //    garage[currentSpot] = string.Empty;

        //    Console.WriteLine($"Vehicle moved from spot {currentSpot + 1} to spot {availableSpot + 1}.");
        //}

        //static void SearchVehicle(string[] garage)
        //{
        //    Console.Write("Enter license plate to search: ");
        //    string? licensePlate = Console.ReadLine();

        //    if (string.IsNullOrEmpty(licensePlate))
        //    {
        //        Console.WriteLine("License plate cannot be empty.");
        //        return;
        //    }

        //    // hitta fordonet
        //    int vehicleSpot = Array.FindIndex(garage, spot => spot?.Contains($"#{licensePlate}") == true);

        //    if (vehicleSpot == -1)
        //    {
        //        Console.WriteLine("Vehicle not found.");
        //        return;
        //    }

        //    // visa fordonsinformation
        //    string[] vehicleData = garage[vehicleSpot].Split('#');
        //    string vehicleType = vehicleData[0];

        //    if (parkingTimes.TryGetValue(licensePlate, out DateTime startTime))
        //    {
        //        TimeSpan duration = DateTime.Now - startTime;
        //        int fee = CalculateParkingCost(duration, vehicleType);

        //        Console.WriteLine($"Vehicle found in spot {vehicleSpot + 1}.");
        //        Console.WriteLine($"Vehicle Type: {vehicleType}");
        //        Console.WriteLine($"License Plate: {licensePlate}");
        //        Console.WriteLine($"Parking Duration: {duration.Hours}h {duration.Minutes}m");
        //        Console.WriteLine($"Current Fee: {fee} CZK");
        //    }
        //    else
        //    {
        //        Console.WriteLine("Error: Parking time not found.");
        //    }
        //}
    }
}
