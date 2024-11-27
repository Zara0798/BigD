using System;
using ParkingSystem.Classes_Folder;
using System.Text.Json;
using System.IO;

namespace ParkingSystem.Classes_Folder
{
    

    public class ParkingGarage
    {
        private ParkingLot parkingLot;
        private ConfigData config;

        private List<ParkingSpot> ParkingSpots;

        public ParkingGarage(int totalSpot)
        {
            var parkingGarage = new ParkingGarage(new ParkingLot(totalSpot), new ConfigData());
        }

        public ParkingGarage(ParkingLot parkingLot, ConfigData config)
        {
            this.parkingLot = parkingLot ?? throw new ArgumentNullException(nameof(parkingLot));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            InitializeGarage();
        }

        private void InitializeGarage()
        {
            
        }

        public void SaveParkingDataToFile(string filePath)
        {
            
        }
    }
}
