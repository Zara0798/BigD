using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using Spectre.Console;
using ParkingSystem.Classes_Folder;
using System.Text.Json.Serialization;
using ParkingSystem.Enums;

namespace MainApp
{
    class Program
    {
        static void Main()
        {
            // konfiguration och parkeringsdata
            Config config = LoadConfigData();

            var parkedVehicles = LoadParkedVehicleData();
            var parkingGarage = new ParkingGarage(config, parkedVehicles); // Initialize with current

            // Exempel på att öka TotalSpots
            config.ParkingConfig.IncreaseTotalSpots(20); // Öka TotalSpots med 20 platser

            while (true)
            {
                Console.Clear();
                DisplayTitle();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.ResetColor();

                Console.WriteLine("\nCurrently parked vehicles:");
                DisplayParkedVehicles(parkingGarage);

                Console.WriteLine("\nMenu Options:");
                Console.WriteLine("1. Park Vehicle");
                Console.WriteLine("2. Retrieve Vehicle");
                Console.WriteLine("3. Move Vehicle");
                Console.WriteLine("4. Search Vehicle by License Plate");
                Console.WriteLine("5. View Current Parking Layout");
                Console.WriteLine("6. Reload Configuration File");
                Console.WriteLine("7. Clear Parking Garage");
                Console.WriteLine("8. Exit");
                Console.ResetColor();

                Console.Write("Select an option: ");
                string? choice = Console.ReadLine();

                if (string.IsNullOrEmpty(choice))
                {
                    Console.WriteLine("Invalid option, please try again.");
                    continue;
                }

                switch (choice)
                {
                    case "1":
                        ParkVehicle(parkingGarage);
                        break;
                    case "2":
                        RetrieveVehicle(parkingGarage);
                        break;
                    case "3":
                       // MoveVehicle(parkingGarage);
                        break;
                    case "4":
                        SearchVehicle(parkingGarage);
                        break;
                    case "5":
                        ViewParkingMap(parkingGarage);
                        break;
                    case "6":
                        config = LoadConfigData();
                        Console.WriteLine("Configuration file has been reloaded.");
                        break;
                    case "7":
                       // ClearParkingGarage(parkingGarage);
                        break;
                    case "8":
                        SaveParkingData(parkingGarage);
                        return;
                    default:
                        Console.WriteLine("Invalid option, please try again.");
                        break;
                }

                SaveParkingData(parkingGarage);
                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
            }
        }

        private const string AdminPassword = "brucelee"; // lösenordet till Admin access för att rensa parkeringen.

        static void ViewParkingMap(ParkingGarage garage)
        {
            var parkingMapInfo = garage.GetParkingMapInfo();

            Console.WriteLine("Current Parking Layout:");
            Console.WriteLine("┌──────┬────────────┬───────────────┐");
            Console.WriteLine("│ Spot │ Status     │ License Plate │");
            Console.WriteLine("├──────┼────────────┼───────────────┤");

            foreach (var info in parkingMapInfo)
            {
                Console.Write("│ {0,-4} │ ", info.SpotNumber);
                Console.ForegroundColor = info.Status == "Occupied" ? ConsoleColor.Red : ConsoleColor.Green;
                Console.Write("{0,-10}", info.Status);
                Console.ResetColor();
                Console.WriteLine(" │ {0,-13} │", info.LicensePlate);
            }

            Console.WriteLine("└──────┴────────────┴───────────────┘");
        }
        static void SaveParkingData(ParkingGarage garage)
        {
            var parkedVehicles = garage.Garage
                .Where(spot => spot.IsOccupied)
                .SelectMany(spot => spot.ParkedVehicles.Select(vehicle => new
                {
                    LicensePlate = vehicle.LicensePlate,
                    ParkedVehicle = new ParkedVehicle
                    {
                        Spot = spot.SpotNumber,
                        Type = (VehicleType)Enum.Parse(typeof(VehicleType), vehicle.GetType().Name, true), // Assuming the class name matches the VehicleType enum
                        StartTime = spot.ParkedTime ?? DateTime.Now,
                        EndTime = null // Assuming the vehicle is still parked
                    }
                }))
                .ToDictionary(x => x.LicensePlate, x => x.ParkedVehicle);

            var parkedDataConfig = new ParkedDataConfig
            {
                ParkedVehicles = parkedVehicles
            };

            string json = JsonSerializer.Serialize(parkedDataConfig, new JsonSerializerOptions { WriteIndented = true });
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ParkedDataConfig.json");
            File.WriteAllText(filePath, json);

            Console.WriteLine("Parking data has been saved to ParkedDataConfig.json.");
        }

        static void RetrieveVehicle(ParkingGarage garage)
        {
            Console.Write("Enter license plate: ");
            string? licensePlate = Console.ReadLine();

            if (string.IsNullOrEmpty(licensePlate))
            {
                Console.WriteLine("License plate cannot be empty.");
                return;
            }

            var (success, vehicleInfo) = garage.RetrieveVehicle(licensePlate);
            if (success && vehicleInfo != null)
            {
                Console.WriteLine($"Vehicle retrieved from spot {vehicleInfo.SpotNumber}.");
                Console.WriteLine($"License Plate: {vehicleInfo.LicensePlate}");
                Console.WriteLine($"Parking Time: {vehicleInfo.ParkingTime} minutes");
                Console.WriteLine($"Parking Fee: {vehicleInfo.CurrentFee} CZK");
            }
            else
            {
                Console.WriteLine("Vehicle not found.");
            }
        }


        static void SearchVehicle(ParkingGarage garage)
        {
            Console.Write("Enter license plate: ");
            string? licensePlate = Console.ReadLine();

            if (string.IsNullOrEmpty(licensePlate))
            {
                Console.WriteLine("License plate cannot be empty.");
                return;
            }

            var vehicleInfo = garage.SearchVehicleByLicensePlate(licensePlate);
            if (vehicleInfo != null)
            {
                Console.WriteLine($"Vehicle found in spot {vehicleInfo.SpotNumber}.");
                Console.WriteLine($"Status: {vehicleInfo.Status}");
                Console.WriteLine($"License Plate: {vehicleInfo.LicensePlate}");
                Console.WriteLine($"Parking Time: {vehicleInfo.ParkingTime} minutes");
                Console.WriteLine($"Current Fee: {vehicleInfo.CurrentFee} CZK");
            }
            else
            {
                Console.WriteLine("Vehicle not found.");
            }
        }



        static int CalculateParkingCost(TimeSpan duration, string vehicleType)
        {
            // första 10 min gratis
            if (duration.TotalMinutes <= 10)
            {
                return 0;
            }

            // minusrera 10 minuter från parkeringstiden
            duration = duration.Subtract(TimeSpan.FromMinutes(10));

            int rate = vehicleType.ToLower() switch
            {
                "car" => 2, // 2 CZK per minute för bilar
                "motorcycle" => 1, // 1 CZK per minut för motorcyklar
                "bus" => 3, // 3 CZK per minut för bussar
                "helicopter" => 100, // 100 CZK per minut för helikoptrar
                "bike" => 0, // gratis för cyklar
                _ => 1 // standard värde
            };
            return (int)(duration.TotalMinutes * rate);
        }

        static Config LoadConfigData()
        {
            // Use a relative path to read the JSON file
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"); 
              string jsonString = File.ReadAllText(jsonFilePath);
            // Deserialize the JSON data to Config object
            return JsonSerializer.Deserialize<Config>(jsonString);
        }

        static ParkedDataConfig LoadParkedVehicleData()
        {
            // Use a relative path to read the JSON file
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ParkedDataConfig.json");
            string jsonString = File.ReadAllText(jsonFilePath);
            // Deserialize the JSON data to Config object
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                AllowTrailingCommas = true
            };
            return JsonSerializer.Deserialize<ParkedDataConfig>(jsonString, options);
        }


        //static void SaveConfig(ConfigData configData) { }

        static void DisplayTitle()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"  
______                            ______          _    _                      _____  
| ___ \                           | ___ \        | |  (_)                    / __  \ 
| |_/ / __ __ _  __ _ _   _  ___  | |_/ /_ _ _ __| | ___ _ __   __ _  __   __`' / /' 
|  __/ '__/ _` |/ _` | | | |/ _ \ |  __/ _` | '__| |/ / | '_ \ / _` | \ \ / /  / /   
| |  | | | (_| | (_| | |_| |  __/ | | | (_| | |  |   <| | | | | (_| |  \ V / ./ /___ 
\_|  |_|  \__,_|\__, |\__,_|\___| \_|  \__,_|_|  |_|\_\_|_| |_|\__, |   \_/  \_____/ 
                 __/ |                                          __/ |                
                |___/                                          |___/                 
");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\nParking Rates:");
            Console.WriteLine("Car: 2 CZK per minute (first 10 minutes free)");
            Console.WriteLine("Motorcycle: 1 CZK per minute (first 10 minutes free)");
            Console.WriteLine("Bus: 3 CZK per minute (first 10 minutes free)");
            Console.WriteLine("Helicopter: 100 CZK per minute (first 10 minutes free)");
            Console.WriteLine("Bike: Free");

            Console.ResetColor();
        }

        static void DisplayParkedVehicles(ParkingGarage garage)
        {
            var parkedVehiclesInfo = garage.GetParkedVehiclesInfo();

            Console.WriteLine("┌──────┬────────────┬───────────────┬────────────────┬──────────────────┐");
            Console.WriteLine("│ Spot │ Status     │ License Plate │ Parking Time   │ Current Fee (CZK)│");
            Console.WriteLine("├──────┼────────────┼───────────────┼────────────────┼──────────────────┤");

            foreach (var info in parkedVehiclesInfo)
            {
                string parkingTime = info.ParkingTime.HasValue ? info.ParkingTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "-";

                string parkingDuration = $"{info.ParkingTime.Value}m";
                var currentFee = $"{info.CurrentFee} CZK";


                Console.Write("│ {0,-4} │ ", info.SpotNumber);
                Console.ForegroundColor = info.Status == "Occupied" ? ConsoleColor.Red : ConsoleColor.Green;
                Console.Write("{0,-10}", info.Status); 
                Console.ResetColor();
                Console.WriteLine(" │ {0,-13} │ {1,-14} │ {2,-16} │", info.LicensePlate, parkingDuration, currentFee);
            }

            Console.WriteLine("└──────┴────────────┴───────────────┴────────────────┴──────────────────┘");
        }


        static void ParkVehicle(ParkingGarage garage)
        {
            Console.Write("Enter vehicle type (Car, Motorcycle, Bus, Bicycle, Helicopter): ");
            string? vehicleTypeInput = Console.ReadLine();

            if (string.IsNullOrEmpty(vehicleTypeInput) || !Enum.TryParse(vehicleTypeInput, true, out VehicleType vehicleType))
            {
                Console.WriteLine("Invalid vehicle type.");
                return;
            }

            Console.Write("Enter license plate: ");
            string? licensePlate = Console.ReadLine();

            if (string.IsNullOrEmpty(licensePlate))
            {
                Console.WriteLine("License plate cannot be empty.");
                return;
            }

            // Check if the vehicle is already parked
            if (garage.Garage.Any(spot => spot.ParkedVehicles.Any(v => v.LicensePlate == licensePlate)))
            {
                Console.WriteLine("This vehicle is already parked.");
                return;
            }

            // Find an available spot
            ParkingSpot? availableSpot = garage.Garage.FirstOrDefault(spot => !spot.IsOccupied && spot.Capacity >= spot.GetVehicleSize(vehicleType));

            if (availableSpot == null)
            {
                Console.WriteLine("No available spots.");
                return;
            }

            // Create the vehicle and park it
            Vehicle vehicle = garage.CreateVehicle(licensePlate, vehicleType);
            vehicle.PricePerHour = garage.GetVehiclePricePerHour(vehicleType);

            garage.ParkVehicle(garage, vehicle, availableSpot.SpotNumber);

            Console.WriteLine($"Vehicle parked in spot {availableSpot.SpotNumber}.");
        }


   
    }

}