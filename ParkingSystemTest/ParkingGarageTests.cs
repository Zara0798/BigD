using ParkingSystem.Classes_Folder;
using ParkingSystem.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingSystemTest
{
    [TestClass]

    public class ParkingGarageTests
    {
        [TestMethod]
        public void ClearParkingGarage_ShouldClearAllParkedVehicles()
        {
            // Arrange
            var config = new Config
            {
                ParkingConfig = new ParkingConfig { TotalSpots = 5 },
                PriceList = new List<PriceList>()
            };

            var parkedDataConfig = new ParkedDataConfig
            {
                ParkedVehicles = new Dictionary<string, ParkedVehicle>
                {
                    { "ABC123", new ParkedVehicle { Spot = 1, StartTime = DateTime.Now.AddHours(-1), Type = VehicleType.Car } },
                    { "XYZ789", new ParkedVehicle { Spot = 2, StartTime = DateTime.Now.AddHours(-2), Type = VehicleType.Motorcycle } }
                }
            };

            var parkingGarage = new ParkingGarage(config, parkedDataConfig);

            // Act
            parkingGarage.ClearParkingGarage();

            // Assert
            foreach (var spot in parkingGarage.Garage)
            {
                Assert.AreEqual(0, spot.ParkedVehicles.Count);
                Assert.AreEqual(0, spot.OccupiedSize);
                Assert.IsFalse(spot.IsOccupied);
            }
        }

    }
}
