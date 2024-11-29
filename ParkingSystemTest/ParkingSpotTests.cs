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
        public class ParkingSpotTests
        {
            [TestMethod]
            public void ParkingSpot_Constructor_ShouldInitializeProperties()
            {
                // Arrange
                int spotNumber = 1;

                // Act
                var parkingSpot = new ParkingSpot(spotNumber);

                // Assert
                Assert.AreEqual(spotNumber, parkingSpot.SpotNumber);
                Assert.AreEqual(1.0, parkingSpot.Capacity); // Default capacity for one car spot
                Assert.AreEqual(0, parkingSpot.OccupiedSize);
                Assert.IsNull(parkingSpot.ParkedTime);
                Assert.IsNotNull(parkingSpot.ParkedVehicles);
                Assert.AreEqual(0, parkingSpot.ParkedVehicles.Count);
            }
        }
}
