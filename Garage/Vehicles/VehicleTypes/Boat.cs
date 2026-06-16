using Garage.Interfaces;
using Garage.ValueTypes;

namespace Garage.Vehicles.VehicleTypes
{
    internal class Boat : Vehicle
    {
        public enum MaterialTypes
        {
            Fiberglass,
            Aluminium,
            Wood,
            Steel
        }

        public MaterialTypes HullMaterial { get; init; }

        public Boat(string make, string model, string color, int numberOfWheels, float engineSize, FuelType typeOfFuel, string registrationNumber, Dimensions dimensions, MaterialTypes hullMaterial) : base("Boat", make, model, color, numberOfWheels, engineSize, typeOfFuel, registrationNumber, dimensions)
        {
            HullMaterial = hullMaterial;
        }

        public static Boat Create(IConsoleUI ui)
        {
            var data = CreateVehicleData<Boat>(ui);

            MaterialTypes material = (MaterialTypes)(ui.GetChoiceInput("Available materials. Make your desired choice:", Enum.GetNames(typeof(MaterialTypes)), "No material types to choose from.") ?? 0);

            return new Boat(
                data.Make,
                data.Model,
                data.Color,
                data.NrOfWheels,
                data.EngineSize,
                data.FuelType,
                data.RegNr,
                data.Dimensions,
                material
                );
        }
    }
}
