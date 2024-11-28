using System;
using ParkingSystem.Classes_Folder;
using System.Text.Json;
using System.IO;
using ParkingSystem.Enums;

namespace ParkingSystem.Classes_Folder
{
    

    public class ParkingGarage
    {
        // TODO: Vi behöver läsa in konfiguration från fil och spara här i något lämpligt objekt
        public Config config;
        public List<ParkingSpot> Garage { get; set; }

        //private Dictionary<string, (int spot, DateTime startTime, VehicleType type)> parkedVehicles;

        //Sedan får parkingGarage ha en konstruktor som ropar på en metod som initialiserar P-huset,
        //läser in konfig-fil och data-fil osv.
        //Där är ni på god väg att få det rätt.
        //Däremot förstår jag inte riktigt varför ni har en separat ParkingManager.

 

        //public ParkingGarage(ParkingLot parkingLot, ConfigData config)
        //{
        //    this.parkingLot = parkingLot ?? throw new ArgumentNullException(nameof(parkingLot));
        //    this.config = config ?? throw new ArgumentNullException(nameof(config));
        //    InitializeGarage();
        //}

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
            
        }

        public void SaveParkingDataToFile(string filePath)
        {
            
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
    }
}
