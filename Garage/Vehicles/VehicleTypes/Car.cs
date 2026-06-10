using Garage.ValueTypes;

namespace Garage.Vehicles.VehicleTypes
{
    internal class Car : Vehicle
    {
        public int NumberOfDoors { get; init; }
        public Car(string make, string model, string color, int numberOfWheels, float engineSize, FuelType typeOfFuel, string registrationNumber, Dimensions dimensions, int numberOfDoors) : base(make, model, color, numberOfWheels, engineSize, typeOfFuel, registrationNumber, dimensions)
        {
            NumberOfDoors = numberOfDoors;
        }
    }
}
