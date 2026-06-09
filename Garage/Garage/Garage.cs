
namespace Garage.Garage
{
    internal class Garage
    {
        public Dictionary<int, ParkingSpot[]> Floors { get; init; }

        public Garage(Dictionary<int, ParkingSpot[]> floors)
        {
            Floors = floors;
        }
    }
}
