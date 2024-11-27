using System;
using System.IO;
using System.Collections.Generic;
using ParkingSystem.Enums;

namespace ParkingSystem.Classes_Folder
{
    //public abstract class Vehicle
    //{
    //    public string LicensePlate { get; set; }
    //    public abstract double Size { get; }
    //    public string Type { get; set; }
    //}
    public class ParkingLot
    {

        private List<ParkingSpot> parkingSpots;
        private Dictionary<string, (int spot, DateTime startTime, VehicleType type)> parkedVehicles;


        //Sedan får ParkingLot ha en konstruktor som ropar på en metod som initialiserar P-huset,
        //läser in konfig-fil och data-fil osv.
        //Där är ni på god väg att få det rätt.
        //Däremot förstår jag inte riktigt varför ni har en separat ParkingManager.
        public ParkingLot(int totalSpots)
        {
            parkingSpots = new List<ParkingSpot>(totalSpots);
            parkedVehicles = new Dictionary<string, (int spot, DateTime startTime, VehicleType type)>();

            for (int i = 0; i < totalSpots; i++)
            {
                parkingSpots.Add(new ParkingSpot(i + 1));
            }
        }

        //public bool ParkVehicle(Vehicle vehicle)
        //{
        //    if (!Enum.TryParse(vehicle.Type, out ParkingSpot.VehicleType vehicleType))
        //    {
        //        return false;
        //    }

        //    int availableSpot = FindAvailableSpot(vehicleType);
        //    if (availableSpot == -1) return false;

        //    var parkingSpotVehicle = new ParkingSpot.Vehicle(vehicle.LicensePlate, vehicleType);

        //    parkingSpots[availableSpot].ParkVehicle(parkingSpotVehicle);
        //    parkedVehicles[vehicle.LicensePlate] = (availableSpot, DateTime.Now, (VehicleType)vehicleType);
        //    return true;
        //}

        private int FindAvailableSpot(VehicleType type)
        {
            for (int i = 0; i < parkingSpots.Count; i++)
            {
                if (!parkingSpots[i].IsOccupied)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool RetrieveVehicle(string licensePlate)
        {
            if (!parkedVehicles.TryGetValue(licensePlate, out var vehicleInfo)) return false;

            parkingSpots[vehicleInfo.spot].ClearSpot();
            parkedVehicles.Remove(licensePlate);
            return true;
        }

        public (int spot, DateTime startTime, VehicleType type)? FindVehicle(string licensePlate)
        {
            if (parkedVehicles.TryGetValue(licensePlate, out var vehicleInfo))
            {
                return vehicleInfo;
            }
            return null;
        }
    }
}