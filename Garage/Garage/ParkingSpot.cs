using Garage.ValueTypes;
using Garage.Vehicles;

namespace Garage.Garage
{
    internal class ParkingSpot
    {
        public Dimensions ParkingSize { get; init; }
        public int SpotNumber { get; init; }
        public bool IsOccupied
        {
            get
            {
                return ParkedVehicle != null;
            }
        }
        public Vehicle? ParkedVehicle { get; private set; }

        public ParkingSpot(double length, double width, double height, int spotNumber)
        {
            ParkingSize = new Dimensions(length, width, height);
            SpotNumber = spotNumber;
        }

        public bool ParkVehicle(Vehicle vehicle)
        {
            if (ParkedVehicle != null)
            {
                Console.WriteLine($"Parking spot {SpotNumber} is already occupied by {ParkedVehicle.Make} {ParkedVehicle.Model}.");
                return false;
            }
            else if (vehicle.Dimensions.Length > ParkingSize.Length || vehicle.Dimensions.Width > ParkingSize.Width || vehicle.Dimensions.Height > ParkingSize.Height)
            {
                Console.WriteLine($"Vehicle {vehicle.Make} {vehicle.Model} does not fit in parking spot {SpotNumber}.");
                return false;
            }

            ParkedVehicle = vehicle;
            return true;
        }
    }
}
