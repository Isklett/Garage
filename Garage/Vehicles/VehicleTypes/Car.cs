using Garage.ValueTypes;

namespace Garage.Vehicles.VehicleTypes
{
    internal class Car : Vehicle
    {
        public Car(string make, string model, int numberOfWheels, float engineSize, FuelType typeOfFuel, string registrationNumber, Dimensions dimensions) : base(make, model, numberOfWheels, engineSize, typeOfFuel, registrationNumber, dimensions)
        {
        }
    }
}
