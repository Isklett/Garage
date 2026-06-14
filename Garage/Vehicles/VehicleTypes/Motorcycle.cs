using Garage.Interfaces;
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

        public static Motorcycle Create(IConsoleUI ui)
        {
            var data = CreateVehicleData(ui);

            int engineDisplacement = ui.GetIntInput("Enter engine displacement in cc:");

            return new Motorcycle(
                data.Make,
                data.Model,
                data.Color,
                data.NrOfWheels,
                data.EngineSize,
                data.FuelType,
                data.RegNr,
                data.Dimensions,
                engineDisplacement
                );
        }
    }
}
