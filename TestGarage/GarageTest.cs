using Garage.Garage;
using Garage.Interfaces;
using Garage.ValueTypes;
using Garage.Vehicles;
using Garage.Vehicles.VehicleTypes;
using Moq;
using static Garage.ValueTypes.Enumerators.VehicleEnums;

namespace TestGarage
{
    public class GarageTests
    {

        private static Dimensions DefaultSpotSize => new Dimensions(5, 3, 2);


        #region Test helper methods
        private static Car CreateCar(
            string make = "Toyota",
            string model = "Corolla",
            string color = "Red",
            int wheels = 4,
            float engineSize = 1.6f,
            FuelType fuelType = FuelType.Gasoline,
            string regNr = "ABC123",
            Dimensions? dimensions = null,
            int doors = 4)
        {
            return new Car(
                make,
                model,
                color,
                wheels,
                engineSize,
                fuelType,
                regNr,
                dimensions ?? new Dimensions(4, 2, 1.5),
                doors);
        }

        private static Garage<ParkingSpot<Vehicle>> CreateGarage(
            string name = "Test Garage",
            int capacity = 3,
            Dimensions? spotSize = null,
            IConsoleUI? ui = null)
        {
            return new Garage<ParkingSpot<Vehicle>>(
                name,
                capacity,
                spotSize ?? DefaultSpotSize,
                ui ?? new Mock<IConsoleUI>().Object);
        }
        #endregion

        [Fact]
        public void Constructor_GivenCapacity_CreatesThatManyEmptySpots()
        {
            // Arrange
            int capacity = 4;

            // Act
            var garage = CreateGarage(capacity: capacity);

            // Assert
            Assert.Equal(capacity, garage.Count);
            Assert.All(garage, spot => Assert.False(spot.IsOccupied));
        }

        [Fact]
        public void Constructor_GivenCapacity_AssignsSequentialSpotNumbers()
        {
            // Arrange
            int capacity = 3;

            // Act
            var garage = CreateGarage(capacity: capacity);

            // Assert
            for (int i = 0; i < capacity; i++)
            {
                Assert.Equal(i, garage[i].SpotNumber);
            }
        }

        [Fact]
        public void Constructor_GivenName_SetsNameProperty()
        {
            // Arrange
            string name = "Downtown Garage";

            // Act
            var garage = CreateGarage(name: name);

            // Assert
            Assert.Equal(name, garage.Name);
        }

        // IsFull checks

        [Fact]
        public void IsFull_WhenAllSpotsOccupied_ReturnsTrue()
        {
            // Arrange
            var garage = CreateGarage(capacity: 2);
            var car1 = CreateCar(regNr: "AAA111");
            var car2 = CreateCar(regNr: "BBB222");

            // Act
            garage.ParkVehicle(garage[0], car1, out _);
            garage.ParkVehicle(garage[1], car2, out _);

            // Assert
            Assert.True(garage.IsFull);
        }

        [Fact]
        public void IsFull_WhenSpotsAreFree_ReturnsFalse()
        {
            // Arrange
            var garage = CreateGarage(capacity: 2);
            var car = CreateCar();

            // Act
            garage.ParkVehicle(garage[0], car, out _);

            // Assert
            Assert.False(garage.IsFull);
        }

        // ParkVehicle

        [Fact]
        public void ParkVehicle_VehicleFitsInEmptySpot_ReturnsTrueAndOccupiesSpot()
        {
            // Arrange
            var garage = CreateGarage();
            var car = CreateCar();
            var spot = garage[0];

            // Act
            bool result = garage.ParkVehicle(spot, car, out string message);

            // Assert
            Assert.True(result);
            Assert.True(spot.IsOccupied);
            Assert.Equal(car, spot.ParkedVehicle);
            Assert.Contains("has been parked", message);
        }

        [Fact]
        public void ParkVehicle_SpotAlreadyOccupied_ReturnsFalseAndKeepsOriginalVehicle()
        {
            // Arrange
            var garage = CreateGarage();
            var firstCar = CreateCar(regNr: "AAA111");
            var secondCar = CreateCar(regNr: "BBB222");
            var spot = garage[0];
            garage.ParkVehicle(spot, firstCar, out _);

            // Act
            bool result = garage.ParkVehicle(spot, secondCar, out string message);

            // Assert
            Assert.False(result);
            Assert.Equal(firstCar, spot.ParkedVehicle);
            Assert.Contains("already occupied", message);
        }

        [Fact]
        public void ParkVehicle_VehicleTooBigForSpot_ReturnsFalseAndSpotRemainsEmpty()
        {
            // Arrange
            var garage = CreateGarage(spotSize: new Dimensions(3, 2, 1.5));
            var oversizedCar = CreateCar(dimensions: new Dimensions(4, 2, 1.5));
            var spot = garage[0];

            // Act
            bool result = garage.ParkVehicle(spot, oversizedCar, out string message);

            // Assert
            Assert.False(result);
            Assert.False(spot.IsOccupied);
            Assert.Contains("does not fit", message);
        }

        // RemoveVehicle

        [Fact]
        public void RemoveVehicle_OccupiedSpot_ReturnsTrueAndClearsSpot()
        {
            // Arrange
            var mockUi = new Mock<IConsoleUI>();
            var garage = CreateGarage(ui: mockUi.Object);
            var car = CreateCar(make: "Volvo", model: "V60");
            var spot = garage[0];
            garage.ParkVehicle(spot, car, out _);

            // Act
            bool result = garage.RemoveVehicle(spot);

            // Assert
            Assert.True(result);
            Assert.False(spot.IsOccupied);
            mockUi.Verify(
                ui => ui.ShowMessage(It.Is<string>(m => m.Contains("Volvo") && m.Contains("V60"))),
                Times.Once);
        }

        [Fact]
        public void RemoveVehicle_EmptySpot_ReturnsFalseAndShowsError()
        {
            // Arrange
            var mockUi = new Mock<IConsoleUI>();
            var garage = CreateGarage(ui: mockUi.Object);
            var spot = garage[0];

            // Act
            bool result = garage.RemoveVehicle(spot);

            // Assert
            Assert.False(result);
            mockUi.Verify(ui => ui.ShowError(It.IsAny<string>()), Times.Once);
        }

        // TryFindVehicle

        [Fact]
        public void TryFindVehicle_RegistrationNumberExists_ReturnsTrueWithCorrectSpot()
        {
            // Arrange
            var garage = CreateGarage(capacity: 3);
            var car = CreateCar(regNr: "XYZ999");
            garage.ParkVehicle(garage[1], car, out _);

            // Act
            bool found = garage.TryFindVehicle("XYZ999", out var spot);

            // Assert
            Assert.True(found);
            Assert.NotNull(spot);
            Assert.Equal(1, spot!.SpotNumber);
        }

        [Fact]
        public void TryFindVehicle_RegistrationNumberDoesNotExist_ReturnsFalseWithNullSpot()
        {
            // Arrange
            var garage = CreateGarage();

            // Act
            bool found = garage.TryFindVehicle("NOTHERE", out var spot);

            // Assert
            Assert.False(found);
            Assert.Null(spot);
        }

        // GetFreeSpots

        [Fact]
        public void GetFreeSpots_ReturnsOnlyUnoccupiedSpotsThatFitGivenDimensions()
        {
            // Arrange
            var garage = CreateGarage(capacity: 3, spotSize: new Dimensions(5, 3, 2));
            var car = CreateCar();
            garage.ParkVehicle(garage[0], car, out _); // occupy spot 0
            var searchDimensions = new Dimensions(4, 2, 1.5); // smaller than spot size on all axes

            // Act
            var freeSpots = garage.GetFreeSpots(searchDimensions);

            // Assert
            Assert.Equal(2, freeSpots.Count);
            Assert.DoesNotContain(garage[0], freeSpots);
        }

        [Fact]
        public void GetFreeSpots_GivenDimensionsTooLargeForAnySpot_ReturnsEmptyList()
        {
            // Arrange
            var garage = CreateGarage(capacity: 2, spotSize: new Dimensions(3, 2, 1.5));
            var tooLargeDimensions = new Dimensions(10, 10, 10);

            // Act
            var freeSpots = garage.GetFreeSpots(tooLargeDimensions);

            // Assert
            Assert.Empty(freeSpots);
        }

        // GetOccupiedSpotList

        [Fact]
        public void GetOccupiedSpotList_ReturnsFormattedEntryForEachOccupiedSpot()
        {
            // Arrange
            var garage = CreateGarage(capacity: 3);
            var car = CreateCar(make: "Honda", model: "Civic");
            garage.ParkVehicle(garage[0], car, out _);

            // Act
            var occupiedList = garage.GetOccupiedSpotList();

            // Assert
            Assert.Single(occupiedList);
            Assert.Contains("Honda", occupiedList[0]);
            Assert.Contains("Civic", occupiedList[0]);
        }

        // GetOccupiedSpots

        [Fact]
        public void GetOccupiedSpots_ReturnsOnlySpotsWithParkedVehicles()
        {
            // Arrange
            var garage = CreateGarage(capacity: 3);
            garage.ParkVehicle(garage[0], CreateCar(regNr: "AAA111"), out _);
            garage.ParkVehicle(garage[2], CreateCar(regNr: "BBB222"), out _);

            // Act
            var occupiedSpots = garage.GetOccupiedSpots().ToList();

            // Assert
            Assert.Equal(2, occupiedSpots.Count);
            Assert.All(occupiedSpots, spot => Assert.True(spot.IsOccupied));
        }

        // SearchVehicles

        [Theory] // Runs the test with both lower and upper case
        [InlineData("xyz999")]
        [InlineData("XYZ999")]
        public void SearchVehicles_ByRegistrationNumber_IsCaseInsensitiveAndReturnsMatch(string searchTerm)
        {
            // Arrange
            var garage = CreateGarage();
            var car = CreateCar(regNr: "XYZ999");
            garage.ParkVehicle(garage[0], car, out _);

            // Act
            var results = garage.SearchVehicles(null, searchTerm, null, null, null, null, null);

            // Assert
            Assert.Single(results);
        }

        [Fact]
        public void SearchVehicles_ByMakeAndFuelType_ReturnsOnlyMatchingVehicles()
        {
            // Arrange
            var garage = CreateGarage(capacity: 3);
            var dieselVolvo = CreateCar(make: "Volvo", regNr: "AAA111", fuelType: FuelType.Diesel);
            var petrolVolvo = CreateCar(make: "Volvo", regNr: "BBB222", fuelType: FuelType.Gasoline);
            var dieselFord = CreateCar(make: "Ford", regNr: "CCC333", fuelType: FuelType.Diesel);
            garage.ParkVehicle(garage[0], dieselVolvo, out _);
            garage.ParkVehicle(garage[1], petrolVolvo, out _);
            garage.ParkVehicle(garage[2], dieselFord, out _);

            // Act
            var results = garage.SearchVehicles(
                null, null, "Volvo", null, null, null, ((int)FuelType.Diesel).ToString());

            // Assert
            Assert.Single(results);
            Assert.Equal("AAA111", results.First().ParkedVehicle!.vehicleData.RegNr);
        }

        [Fact]
        public void SearchVehicles_NoFiltersProvided_ReturnsAllOccupiedVehicles()
        {
            // Arrange
            var garage = CreateGarage(capacity: 2);
            garage.ParkVehicle(garage[0], CreateCar(regNr: "AAA111"), out _);
            garage.ParkVehicle(garage[1], CreateCar(regNr: "BBB222"), out _);

            // Act
            var results = garage.SearchVehicles(null, null, null, null, null, null, null);

            // Assert
            Assert.Equal(2, results.Count());
        }

        [Fact]
        public void SearchVehicles_GivenEmptySpots_ExcludesThemFromResults()
        {
            // Arrange
            var garage = CreateGarage(capacity: 3);
            garage.ParkVehicle(garage[0], CreateCar(regNr: "AAA111"), out _);
            // spots 1 and 2 remain empty

            // Act
            var results = garage.SearchVehicles(null, null, null, null, null, null, null);

            // Assert
            Assert.Single(results);
        }

        // Enumerator

        [Fact]
        public void GetEnumerator_IteratesOverAllSpotsInGarage()
        {
            // Arrange
            var garage = CreateGarage(capacity: 4);

            // Act
            int count = 0;
            foreach (var spot in garage)
            {
                count++;
            }

            // Assert
            Assert.Equal(4, count);
        }

        [Fact]
        public void Indexer_IndexOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var garage = CreateGarage(capacity: 2);

            // Act
            Action act = () => { var spot = garage[5]; };

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(act);
        }
    }
}