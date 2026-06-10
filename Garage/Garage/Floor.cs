

using Garage.Vehicles;

namespace Garage.Garage
{
    internal class Floor
    {
        public int Level { get; }

        private readonly ParkingSpot[] _spots;

        public Floor(int level, int nrOfSpots)
        {
            Level = level;
            _spots = new ParkingSpot[nrOfSpots];
        }

        public Vehicle? GetVehicleAtSpot(int spotNumber)
        {
            if (spotNumber < 0 || spotNumber >= _spots.Length)
            {
                Console.WriteLine($"{spotNumber} is an invalid parking spot number.");
                return null;
            }
            return _spots[spotNumber]?.ParkedVehicle;
        }
    }
}
