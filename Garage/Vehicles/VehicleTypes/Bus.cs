using Garage.Interfaces;
using Garage.ValueTypes;

namespace Garage.Vehicles.VehicleTypes
{
    internal class Bus : Vehicle
    {
        public int PassengerCapacity { get; init; }
        public Bus(string make, string model, string color, int numberOfWheels, float engineSize, FuelType typeOfFuel, string registrationNumber, Dimensions dimensions, int passengerCapacity) : base("Bus", make, model, color, numberOfWheels, engineSize, typeOfFuel, registrationNumber, dimensions)
        {
            PassengerCapacity = passengerCapacity;
        }

        public static Bus Create(IConsoleUI ui, List<string> parkedRegNrs)
        {
            var data = CreateVehicleData<Bus>(ui, parkedRegNrs);

            int passengerCapacity = ui.GetIntInput("Enter passenger capacity:");

            return new Bus(
                data.Make,
                data.Model,
                data.Color,
                data.NrOfWheels,
                data.EngineSize,
                data.FuelType,
                data.RegNr,
                data.Dimensions,
                passengerCapacity
                );
        }
    }
}
