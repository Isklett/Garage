using Garage.ValueTypes;

namespace Garage.Vehicles.VehicleTypes
{
    internal class Airplane : Vehicle
    {
        public double Wingspan { get; init; }

        public Airplane(string make, string model, string color, int numberOfWheels, float engineSize, FuelType typeOfFuel, string registrationNumber, Dimensions dimensions, double wingspan) : base(make, model, color, numberOfWheels, engineSize, typeOfFuel, registrationNumber, dimensions)
        {
            Wingspan = wingspan;
        }
    }
}
