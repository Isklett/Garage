using Garage.ValueTypes;

namespace Garage.Vehicles.VehicleTypes.CarTypes
{
    internal class SUV : Car
    {
        public SUV(string make, string model, string color, int numberOfWheels, float engineSize, FuelType typeOfFuel, string registrationNumber, Dimensions dimensions, int numberOfDoors) : base(make, model, color, numberOfWheels, engineSize, typeOfFuel, registrationNumber, dimensions, numberOfDoors)
        {
        }
    }
}
