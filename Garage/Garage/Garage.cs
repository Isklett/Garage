
using Garage.Interfaces;
using Garage.Vehicles;
using System.Collections;
using System.Linq;

namespace Garage.Garage
{
    internal class Garage<T> : ILimitedList<T> where T : ParkingSpot
    {
        private List<T> _spots;
        private int _capacity;
        private readonly IConsoleUI _ui;

        public int Count => _spots.Count;

        public bool IsFull => _capacity <= Count;

        public T this[int index] => _spots[index];

        public Garage(int nrOfSpots, IConsoleUI ui)
        {
            _capacity = nrOfSpots;
            _spots = new List<T>(_capacity);
            _ui = ui;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T item in _spots)
            {
                yield return item;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<T> GetOccupiedSpots()
        {
            foreach (T spot in _spots)
            {
                if (spot.IsOccupied)
                {
                    yield return spot;
                }
            }
        }
        public List<string> GetOccupiedSpotList()
        {
            return _spots
                .Where(s => s != null && s.IsOccupied)
                .Select(s => $"{s.SpotNumber}: {s.ParkedVehicle?.Make} {s.ParkedVehicle?.Model}")
                .ToList();
        }

        public bool ParkVehicle(T parkingSpot, Vehicle vehicle)
        {
            if (parkingSpot.ParkVehicle(vehicle))
            {
                _ui.ShowMessage($"Vehicle {vehicle.Make} {vehicle.Model} parked in spot {parkingSpot.SpotNumber}.");
                return true;
            }
            else
            {
                _ui.ShowError($"Failed to park vehicle {vehicle.Make} {vehicle.Model} in spot {parkingSpot.SpotNumber}.");
                return false;
            }
        }

        public bool RemoveVehicle(T parkingSpot)
        {
            if (parkingSpot.IsOccupied)
            {
                _ui.ShowMessage($"Vehicle {parkingSpot.ParkedVehicle?.Make} {parkingSpot.ParkedVehicle?.Model} removed from spot {parkingSpot.SpotNumber}.");
                parkingSpot.RemoveVehicle();
                return true;
            }
            else
            {
                _ui.ShowError($"Parking spot {parkingSpot.SpotNumber} is already empty.");
                return false;
            }
        }
        public bool RemoveVehicle(T parkingSpot, Vehicle vehicle)
        {
            if (parkingSpot.ParkedVehicle == vehicle)
            {
                if (parkingSpot.IsOccupied)
                {
                    _ui.ShowMessage($"Vehicle {parkingSpot.ParkedVehicle?.Make} {parkingSpot.ParkedVehicle?.Model} removed from spot {parkingSpot.SpotNumber}.");
                    parkingSpot.RemoveVehicle();
                    return true;
                }
                else
                {
                    _ui.ShowError($"Parking spot {parkingSpot.SpotNumber} is already empty.");
                    return false;
                }
            }
            else
            {
                _ui.ShowMessage($"{vehicle.Make} {vehicle.Model} is not parked in this spot.");
                return false;
            }
        }

        public bool Add(T item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }
    }
}
