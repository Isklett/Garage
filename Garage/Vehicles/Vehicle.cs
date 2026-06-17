using Garage.Interfaces;
using Garage.ValueTypes;
using System.Runtime.CompilerServices;

namespace Garage.Vehicles
{
    internal abstract class Vehicle : IParkable
    {

        public readonly record struct VehicleData
            (
            string VehicleType,
            string RegNr,
            string Make,
            string Model,
            string Color,
            int NrOfWheels,
            float EngineSize,
            FuelType FuelType,
            Dimensions Dimensions
            );

        public VehicleData vehicleData { get; init; }

        public DateTime ArrivalTime { get; set; }

        public Vehicle(string vehicleType, string make, string model, string color, int numberOfWheels, float engineSize, FuelType typeOfFuel, string registrationNumber, Dimensions dimensions)
        {
            vehicleData = new VehicleData(vehicleType, registrationNumber, make, model, color, numberOfWheels, engineSize, typeOfFuel, dimensions);
        }

        public bool ParkVehicle()
        {
            ArrivalTime = DateTime.Now;
            return true;
        }

        protected static VehicleData CreateVehicleData<T>(IConsoleUI ui, List<string> parkedRegNrs)
        {
            string regNr = ui.GetStringInput("Enter registration number:");
            while (parkedRegNrs.Contains(regNr))
            {
                regNr = ui.GetStringInput($"{regNr} is already parked in this garage. Enter registration number:");
            }
            string make = ui.GetStringInput("Enter make:");
            string model = ui.GetStringInput("Enter model:");
            string color = ui.GetStringInput("Enter color:");
            int nrOfWheels = ui.GetIntInput("Enter number of wheels:");
            float engineSize = ui.GetFloatInput("Enter engine size in liters:");
            int typeOfFuelChoice = ui.GetChoiceInput("What fuel type? Make your desired choice:", Enum.GetNames(typeof(FuelType)), "No fuel types to choose from.") ?? 0;
            FuelType typeOfFuel = (FuelType)typeOfFuelChoice;
            double length = ui.GetDoubleInput("Enter length:");
            double width = ui.GetDoubleInput("Enter width:");
            double height = ui.GetDoubleInput("Enter height:");

            return new VehicleData(typeof(T).Name, regNr, make, model, color, nrOfWheels, engineSize, typeOfFuel, new Dimensions(length, width, height));
        }
    }
}
