using Garage.ValueTypes;

namespace Garage.Vehicles.VehicleTypes.CarTypes
{
    internal class Sedan : Car
    {
        public Sedan(string make, string model, string color, int numberOfWheels, float engineSize, FuelType typeOfFuel, string registrationNumber, Dimensions dimensions, int numberOfDoors) : base(make, model, color, numberOfWheels, engineSize, typeOfFuel, registrationNumber, dimensions, numberOfDoors)
        {
        }
    }
}
