using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using Spectre.Console;
using ParkingSystem.Classes_Folder;

namespace MainApp
{
    class Program
    {
        static void Main()
        {
            // konfiguration och parkeringsdata
            ConfigData config = LoadConfigData();
            var parkingGarage1 = new ParkingGarage(config.TotalSpots); // Initialize with current TotalSpots
            var parkingGarage = new string[config.TotalSpots];// LoadParkingData();

            // Exempel på att öka TotalSpots
            config.IncreaseTotalSpots(20); // Öka TotalSpots med 20 platser

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
                        MoveVehicle(parkingGarage);
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
                        ClearParkingGarage(parkingGarage);
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

        class ConfigData
        {
            public int TotalSpots { get; set; }
            public int VIPSpots { get; set; }
            public List<Vehicle> Vehicles { get; set; }

            public void IncreaseTotalSpots(int additionalSpots)
            {
                TotalSpots += additionalSpots;
            }
        }

        public class Vehicle
        {
            public string Type { get; set; } 
            public double RequiredSpots { get; set; } 
        }

        //static ConfigData LoadConfigData()
        //{
        //    return new ConfigData
        //    {
        //        TotalSpots = 100,
        //        VIPSpots = 10
        //    };
        //}

        private const string AdminPassword = "brucelee"; // lösenordet till Admin access för att rensa parkeringen.

        class ParkingData
        {
            public string[] Garage { get; set; } = Array.Empty<string>();
            public Dictionary<string, DateTime> ParkingTimes { get; set; } = new Dictionary<string, DateTime>();
        }

        static void ClearParkingGarage(string[] garage)
        {
            Console.Write("Enter admin password to clear the parking garage: ");
            string? password = Console.ReadLine();

            if (password == AdminPassword)
            {
                Array.Clear(garage, 0, garage.Length);
                parkingTimes.Clear();
                Console.WriteLine("Parking garage has been cleared.");
            }
            else
            {
                Console.WriteLine("Incorrect password. Access denied.");
            }
        }

        static double GetRequiredSpots(string vehicleType)
        {
            return vehicleType.ToLower() switch
            {
                "car" => 1,
                "motorcycle" => 0.5,
                "bus" => 3,
                "helicopter" => 5,
                "bike" => 0.2,
                _ => 1 // standard för 1 parkeringsplats
            };
        }

        static void ParkVehicle(string[] garage)
        {
            Console.Write("Enter vehicle type: ");
            string? vehicleType = Console.ReadLine();

            if (string.IsNullOrEmpty(vehicleType))
            {
                Console.WriteLine("Vehicle type cannot be empty.");
                return;
            }

            Console.Write("Enter license plate: ");
            string? licensePlate = Console.ReadLine();

            if (string.IsNullOrEmpty(licensePlate))
            {
                Console.WriteLine("License plate cannot be empty.");
                return;
            }

            // kontrollerar om fordon redan är parkerat
            if (parkingTimes.ContainsKey(licensePlate))
            {
                Console.WriteLine("This vehicle is already parked.");
                return;
            }

            double requiredSpots = GetRequiredSpots(vehicleType);

            // skriv ut nuvarande status för garaget
            Console.WriteLine("Current state of the garage:");
            for (int i = 0; i < garage.Length; i++)
            {
                Console.WriteLine($"Spot {i + 1}: {garage[i]}");
            }

            // tillgängliga platser
            int availableSpot = FindAvailableSpots(garage, requiredSpots);

            if (availableSpot == -1)
            {
                Console.WriteLine("No available spots.");
                return;
            }

            // parkera fordonet
            for (int i = 0; i < (int)Math.Ceiling(requiredSpots); i++)
            {
                garage[availableSpot + i] = $"{vehicleType}#{licensePlate}";
            }
            parkingTimes[licensePlate] = DateTime.Now;

            Console.WriteLine($"Vehicle parked in spots {availableSpot + 1} to {availableSpot + (int)Math.Ceiling(requiredSpots)}.");

            // uppdaterad lägesstatus
            Console.WriteLine("Updated state of the garage:");
            for (int i = 0; i < garage.Length; i++)
            {
                Console.WriteLine($"Spot {i + 1}: {garage[i]}");
            }
        }

        static int FindAvailableSpots(string[] garage, double requiredSpots)
        {
            int intRequiredSpots = (int)Math.Ceiling(requiredSpots);

            for (int i = 0; i <= garage.Length - intRequiredSpots; i++)
            {
                bool allSpotsAvailable = true;
                for (int j = 0; j < intRequiredSpots; j++)
                {
                    if (!string.IsNullOrEmpty(garage[i + j]))
                    {
                        allSpotsAvailable = false;
                        break;
                    }
                }
                if (allSpotsAvailable)
                {
                    Console.WriteLine($"Available spot found at index {i} for {intRequiredSpots} spots.");
                    return i;
                }
            }
            Console.WriteLine("No available spots found.");
            return -1;
        }

        static void RetrieveVehicle(string[] garage)
        {
            Console.Write("Enter license plate: ");
            string? licensePlate = Console.ReadLine();

            if (string.IsNullOrEmpty(licensePlate))
            {
                Console.WriteLine("License plate cannot be empty.");
                return;
            }

            // Hitta fordon
            int vehicleSpot = Array.FindIndex(garage, spot => spot?.Contains($"#{licensePlate}") == true);

            if (vehicleSpot == -1)
            {
                Console.WriteLine("Vehicle not found.");
                return;
            }

            // logga nuvarande parkeringstider
            Console.WriteLine("Current parking times:");
            foreach (var entry in parkingTimes)
            {
                Console.WriteLine($"License Plate: {entry.Key}, Start Time: {entry.Value}");
            }

            // räkna parkerings avgift
            string[] vehicleData = garage[vehicleSpot].Split('#');
            string vehicleType = vehicleData[0];

            if (parkingTimes.TryGetValue(licensePlate, out DateTime startTime))
            {
                TimeSpan duration = DateTime.Now - startTime;
                int fee = CalculateParkingCost(duration, vehicleType);

                // ta bort fordonet
                for (int i = 0; i < (int)Math.Ceiling(GetRequiredSpots(vehicleType)); i++)
                {
                    garage[vehicleSpot + i] = string.Empty;
                }
                parkingTimes.Remove(licensePlate);

                Console.WriteLine($"Vehicle retrieved from spot {vehicleSpot + 1}. Parking fee: {fee} CZK.");
            }
            else
            {
                Console.WriteLine("Error: Parking time not found.");
            }
        }

        static void MoveVehicle(string[] garage)
        {
            Console.Write("Enter license plate of the vehicle to move: ");
            string? licensePlate = Console.ReadLine();

            if (string.IsNullOrEmpty(licensePlate))
            {
                Console.WriteLine("License plate cannot be empty.");
                return;
            }

            // hitta fordonet
            int currentSpot = Array.FindIndex(garage, spot => spot?.Contains($"#{licensePlate}") == true);

            if (currentSpot == -1)
            {
                Console.WriteLine("Vehicle not found.");
                return;
            }

            // hitta tillgänglig plats
            int availableSpot = Array.FindIndex(garage, spot => string.IsNullOrEmpty(spot));

            if (availableSpot == -1)
            {
                Console.WriteLine("No available spots.");
                return;
            }

            // flytta fordonet
            garage[availableSpot] = garage[currentSpot];
            garage[currentSpot] = string.Empty;

            Console.WriteLine($"Vehicle moved from spot {currentSpot + 1} to spot {availableSpot + 1}.");
        }

        static void SearchVehicle(string[] garage)
        {
            Console.Write("Enter license plate to search: ");
            string? licensePlate = Console.ReadLine();

            if (string.IsNullOrEmpty(licensePlate))
            {
                Console.WriteLine("License plate cannot be empty.");
                return;
            }

            // hitta fordonet
            int vehicleSpot = Array.FindIndex(garage, spot => spot?.Contains($"#{licensePlate}") == true);

            if (vehicleSpot == -1)
            {
                Console.WriteLine("Vehicle not found.");
                return;
            }

            // visa fordonsinformation
            string[] vehicleData = garage[vehicleSpot].Split('#');
            string vehicleType = vehicleData[0];

            if (parkingTimes.TryGetValue(licensePlate, out DateTime startTime))
            {
                TimeSpan duration = DateTime.Now - startTime;
                int fee = CalculateParkingCost(duration, vehicleType);

                Console.WriteLine($"Vehicle found in spot {vehicleSpot + 1}.");
                Console.WriteLine($"Vehicle Type: {vehicleType}");
                Console.WriteLine($"License Plate: {licensePlate}");
                Console.WriteLine($"Parking Duration: {duration.Hours}h {duration.Minutes}m");
                Console.WriteLine($"Current Fee: {fee} CZK");
            }
            else
            {
                Console.WriteLine("Error: Parking time not found.");
            }
        }

        static void ViewParkingMap(string[] garage)
        {
            Console.WriteLine("Current Parking Layout:");
            Console.WriteLine("┌──────┬────────────┬───────────────┐");
            Console.WriteLine("│ Spot │ Status     │ License Plate │");
            Console.WriteLine("├──────┼────────────┼───────────────┤");

            for (int i = 0; i < garage.Length; i++)
            {
                string status = "Empty";
                string licensePlate = "-";

                if (!string.IsNullOrEmpty(garage[i]))
                {
                    string[] vehicleData = garage[i].Split('#');
                    status = "Occupied";
                    licensePlate = vehicleData[1];
                }

                // visa rad baserad på status och färg
                Console.Write("│ {0,-4} │ ", i + 1);
                Console.ForegroundColor = status == "Occupied" ? ConsoleColor.Red : ConsoleColor.Green;
                Console.Write("{0,-10}", status);
                Console.ResetColor();
                Console.WriteLine(" │ {0,-13} │", licensePlate);
            }
            Console.WriteLine("└──────┴────────────┴───────────────┘");
        }

        static string FilePath = "parkingData.json";
        static ConfigData config = new ConfigData();
        static Dictionary<string, DateTime> parkingTimes = new Dictionary<string, DateTime>();

        static void SaveParkingData(string[] garage)
        {
            var parkingData = new
            {
                Garage = garage,
                ParkingTimes = parkingTimes
            };

            string json = JsonSerializer.Serialize(parkingData);
            File.WriteAllText(FilePath, json);
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

        static string[] LoadParkingData()
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                var parkingData = JsonSerializer.Deserialize<ParkingData>(json);

                if (parkingData != null)
                {
                    parkingTimes = parkingData.ParkingTimes;
                    return parkingData.Garage;
                }
            }

            // Returnera en standardmatris som initierats med tomma strängar om inga data hittas
            return Enumerable.Repeat(string.Empty, config.TotalSpots).ToArray();
        }

        static ConfigData LoadConfigData()
        {
            // Use a relative path to read the JSON file
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"); 
              string jsonString = File.ReadAllText(jsonFilePath);
            // Deserialize the JSON data to Config object
            return JsonSerializer.Deserialize<ConfigData>(jsonString);
        }


        static void SaveConfig(ConfigData configData) { }

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

        static void DisplayParkedVehicles(string[] garage)
        {
            Console.WriteLine("┌──────┬────────────┬───────────────┬────────────────┬──────────────────┐");
            Console.WriteLine("│ Spot │ Status     │ License Plate │ Parking Time   │ Current Fee (CZK)│");
            Console.WriteLine("├──────┼────────────┼───────────────┼────────────────┼──────────────────┤");

            for (int i = 0; i < garage.Length; i++)
            {
                string status = "Empty";
                string licensePlate = "-";
                string parkingDuration = "-";
                string currentFee = "-";

                if (!string.IsNullOrEmpty(garage[i]))
                {
                    string[] vehicleData = garage[i].Split('#');
                    string vehicleType = vehicleData[0];
                    string registration = vehicleData[1];

                    status = "Occupied";
                    licensePlate = registration;

                    if (parkingTimes.TryGetValue(registration, out DateTime startTime))
                    {
                        TimeSpan duration = DateTime.Now - startTime;
                        parkingDuration = $"{duration.Hours}h {duration.Minutes}m";
                        currentFee = $"{CalculateParkingCost(duration, vehicleType)} CZK";
                    }
                }

                // Visa rad med färger baserat på beläggningsstatus
                Console.Write("│ {0,-4} │ ", i + 1);
                Console.ForegroundColor = status == "Occupied" ? ConsoleColor.Red : ConsoleColor.Green;
                Console.Write("{0,-10}", status);
                Console.ResetColor();
                Console.WriteLine(" │ {0,-13} │ {1,-14} │ {2,-16} │", licensePlate, parkingDuration, currentFee);
            }
            Console.WriteLine("└──────┴────────────┴───────────────┴────────────────┴──────────────────┘");
        }
    }
}