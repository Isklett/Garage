using Garage.Interfaces;
using Garage.ValueTypes;
using System.ComponentModel;

namespace Garage.Vehicles
{
    internal class Vehicle : IParkable
    {
        public enum FuelType
        {
            Gasoline,
            Diesel,
            Electric,
            Hybrid
        }

        public string Make { get; init; }
        public string Model { get; init; }
        public string Color { get; init; }
        public int NumberOfWheels { get; init; }
        [Description("Engine size in liters")]
        public float EngineSize { get; init; }
        public FuelType TypeOfFuel { get; init; }

        public string RegistrationNumber { get; }

        public Dimensions Dimensions { get; }

        public DateTime ArrivalTime { get; set; }

        public Vehicle(string make, string model, string color, int numberOfWheels, float engineSize, FuelType typeOfFuel, string registrationNumber, Dimensions dimensions)
        {
            Make = make;
            Model = model;
            Color = color;
            NumberOfWheels = numberOfWheels;
            EngineSize = engineSize;
            TypeOfFuel = typeOfFuel;
            RegistrationNumber = registrationNumber;
            Dimensions = dimensions;
        }

        public bool ParkVehicle()
        {
            ArrivalTime = DateTime.Now;
            return true;
        }
    }
}
