using Garage.ValueTypes;

namespace Garage.Vehicles.VehicleTypes.CarTypes
{
    internal class SUV : Car
    {
        public SUV(string make, string model, int numberOfWheels, float engineSize, FuelType typeOfFuel, string registrationNumber, Dimensions dimensions) : base(make, model, numberOfWheels, engineSize, typeOfFuel, registrationNumber, dimensions)
        {
        }
    }
}
