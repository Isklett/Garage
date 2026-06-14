

using Garage.Vehicles;
using System.Collections;

namespace Garage.Garage
{
    internal class Floor<T> : IEnumerable<T> where T : ParkingSpot<Vehicle>
    {
        public int Level { get; }

        private readonly ParkingSpot<Vehicle>[] _spots;

        public Floor(int level, int nrOfSpots)
        {
            Level = level;
            _spots = new ParkingSpot<Vehicle>[nrOfSpots];
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

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
