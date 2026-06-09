using Garage.ValueTypes;

namespace Garage.Garage
{
    internal class ParkingSpot
    {
        public Dimensions ParkingSize { get; init; }
        public int SpotNumber { get; init; }
        public bool IsOccupied { get; private set; }

        public ParkingSpot(double length, double width, int spotNumber)
        {
            ParkingSize = new Dimensions(length, width);
            SpotNumber = spotNumber;
            IsOccupied = false;
        }

        public void ParkVehicle() => IsOccupied = true;
    }
}
