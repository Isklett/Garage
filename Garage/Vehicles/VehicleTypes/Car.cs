using Garage.Interfaces;
using Garage.ValueTypes;

namespace Garage.Vehicles.VehicleTypes
{
    internal class Car : Vehicle
    {
        public int NumberOfDoors { get; init; }
        public Car(string make, string model, string color, int numberOfWheels, float engineSize, FuelType typeOfFuel, string registrationNumber, Dimensions dimensions, int numberOfDoors) : base("Car", make, model, color, numberOfWheels, engineSize, typeOfFuel, registrationNumber, dimensions)
        {
            NumberOfDoors = numberOfDoors;
        }

        public static Car Create(IConsoleUI ui, List<string> parkedRegNrs)
        {
            var data = CreateVehicleData<Car>(ui, parkedRegNrs);

            int numberOfDoors = ui.GetIntInput("Enter number of doors:");

            return new Car(
                data.Make,
                data.Model,
                data.Color,
                data.NrOfWheels,
                data.EngineSize,
                data.FuelType,
                data.RegNr,
                data.Dimensions,
                numberOfDoors
                );
        }
    }
}
