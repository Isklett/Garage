using Garage.ValueTypes;

namespace Garage.Vehicles.VehicleTypes
{
    internal class Bus : Vehicle
    {
        public int PassengerCapacity { get; init; }
        public Bus(string make, string model, string color, int numberOfWheels, float engineSize, FuelType typeOfFuel, string registrationNumber, Dimensions dimensions, int passengerCapacity) : base(make, model, color, numberOfWheels, engineSize, typeOfFuel, registrationNumber, dimensions)
        {
            PassengerCapacity = passengerCapacity;
        }
    }
}
