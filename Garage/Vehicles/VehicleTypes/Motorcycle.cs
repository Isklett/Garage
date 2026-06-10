using Garage.ValueTypes;

namespace Garage.Vehicles.VehicleTypes
{
    internal class Motorcycle : Vehicle
    {
        public int EngineDisplacement { get; init; } //cc

        public Motorcycle(string make, string model, string color, int numberOfWheels, float engineSize, FuelType typeOfFuel, string registrationNumber, Dimensions dimensions, int engineDisplacement) : base(make, model, color, numberOfWheels, engineSize, typeOfFuel, registrationNumber, dimensions)
        {
            EngineDisplacement = engineDisplacement;
        }
    }
}
