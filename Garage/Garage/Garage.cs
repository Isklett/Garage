using Garage.Interfaces;
using Garage.ValueTypes;
using Garage.Vehicles;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using static Garage.ValueTypes.Enumerators.VehicleEnums;

namespace Garage.Garage
{
    internal class Garage<T> : ILimitedList<T> where T : ParkingSpot<Vehicle>
    {
        private List<T> _spots;
        private string _name;
        private int _capacity;
        private readonly IConsoleUI _ui;

        public int Count => _spots.Count;
        public string Name => _name;

        public bool IsFull => _capacity <= Count;

        public T this[int index] => _spots[index];

        public Garage(string name, int nrOfSpots, Dimensions parkingSpotSize, IConsoleUI ui)
        {
            _name = name;
            _capacity = nrOfSpots;
            _spots = new List<T>(_capacity);
            for (int i = 0; i < _capacity; i++)
            {
                _spots.Add((T)new ParkingSpot<Vehicle>(parkingSpotSize.Length, parkingSpotSize.Width, parkingSpotSize.Height, i));
            }
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

        public IEnumerable<T> GetOccupiedSpots()
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
                .Select(s => $"{s.SpotNumber}: {s.ParkedVehicle?.vehicleData.Make} {s.ParkedVehicle?.vehicleData.Model}")
                .ToList();
        }

        public List<T> GetFreeSpots(Dimensions dimensions)
        {
            return _spots
                .Where(s => !s.IsOccupied && s.ParkingSize.CanFit(dimensions))
                .ToList();
        }

        public bool TryFindVehicle(string registrationNumber, [NotNullWhen(true)] out T? parkingSpot)
        {
            foreach (T spot in _spots)
            {
                if (spot.IsOccupied && spot.ParkedVehicle?.vehicleData.RegNr == registrationNumber)
                {
                    parkingSpot = spot;
                    return true;
                }
            }
            parkingSpot = null;
            return false;
        }

        public IEnumerable<T> SearchVehicles(string? registrationNumber, string? make, string? model, string? color, int? nrOfWheels, FuelType? fuelType)
        {
            return _spots.Where
                    (spot => spot.IsOccupied && spot.ParkedVehicle != null &&
                    (string.IsNullOrWhiteSpace(registrationNumber) || spot.ParkedVehicle.vehicleData.RegNr == registrationNumber) &&
                    (string.IsNullOrWhiteSpace(make) || spot.ParkedVehicle.vehicleData.Make == make) &&
                    (string.IsNullOrWhiteSpace(model) || spot.ParkedVehicle.vehicleData.Model == model) &&
                    (string.IsNullOrWhiteSpace(color) || spot.ParkedVehicle.vehicleData.Color == color) &&
                    (nrOfWheels == null || spot.ParkedVehicle.vehicleData.NrOfWheels == nrOfWheels) &&
                    (fuelType == null || spot.ParkedVehicle.vehicleData.FuelType == fuelType)
                    );
        }

        public bool ParkVehicle(T parkingSpot, Vehicle vehicle)
        {
            if (parkingSpot.ParkVehicle(vehicle))
            {
                _ui.ShowMessage($"Vehicle {vehicle.vehicleData.Make} {vehicle.vehicleData.Model} parked in spot {parkingSpot.SpotNumber}.");
                return true;
            }
            else
            {
                _ui.ShowError($"Failed to park vehicle {vehicle.vehicleData.Make} {vehicle.vehicleData.Model} in spot {parkingSpot.SpotNumber}.");
                return false;
            }
        }

        public bool RemoveVehicle(T parkingSpot)
        {
            if (parkingSpot.IsOccupied)
            {
                _ui.ShowMessage($"Vehicle {parkingSpot.ParkedVehicle?.vehicleData.Make} {parkingSpot.ParkedVehicle?.vehicleData.Model} removed from spot {parkingSpot.SpotNumber}.");
                parkingSpot.RemoveVehicle();
                return true;
            }
            else
            {
                _ui.ShowError($"Parking spot {parkingSpot.SpotNumber} is already empty.");
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
