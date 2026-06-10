using Garage.ValueTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Garage.Vehicles.VehicleTypes
{
    internal class Boat : Vehicle
    {
        public enum MaterialTypes
        {
            Fiberglass,
            Aluminum,
            Wood,
            Steel
        }

        public MaterialTypes HullMaterial { get; init; }

        public Boat(string make, string model, string color, int numberOfWheels, float engineSize, FuelType typeOfFuel, string registrationNumber, Dimensions dimensions, MaterialTypes hullMaterial) : base(make, model, color, numberOfWheels, engineSize, typeOfFuel, registrationNumber, dimensions)
        {
            HullMaterial = hullMaterial;
        }
    }
}
