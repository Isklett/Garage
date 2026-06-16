using Garage.Interfaces;
using Garage.ValueTypes;
using Garage.Vehicles;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Garage.Garage
{
    internal sealed class Garage<T> : ILimitedList<T> where T : ParkingSpot<Vehicle>
    {
        private List<T> _spots;
        private string _name;
        private int _capacity;
        private readonly IConsoleUI _ui;

        public int Count => _spots.Count;
        public string Name => _name;

        public bool IsFull
        {
            get 
            {
                foreach (var item in _spots)
                {
                    if(!item.IsOccupied)
                        return false;
                }
                return true;
            }
        }

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

        public IEnumerable<T> SearchVehicles(string? vehicleType, string? registrationNumber, string? make, string? model, string? color, string? nrOfWheels, string? fuelType)
        {
            int.TryParse(nrOfWheels, out int wheels);
            int.TryParse(fuelType, out int fuel);
            var list = _spots.Where
                    (spot => spot.IsOccupied && spot.ParkedVehicle != null &&
                    (string.IsNullOrWhiteSpace(vehicleType) || spot.ParkedVehicle.vehicleData.VehicleType.ToUpper() == vehicleType.ToUpper()) &&
                    (string.IsNullOrWhiteSpace(registrationNumber) || spot.ParkedVehicle.vehicleData.RegNr.ToUpper() == registrationNumber.ToUpper()) &&
                    (string.IsNullOrWhiteSpace(make) || spot.ParkedVehicle.vehicleData.Make.ToUpper() == make.ToUpper()) &&
                    (string.IsNullOrWhiteSpace(model) || spot.ParkedVehicle.vehicleData.Model.ToUpper() == model.ToUpper()) &&
                    (string.IsNullOrWhiteSpace(color) || spot.ParkedVehicle.vehicleData.Color.ToUpper() == color.ToUpper()) &&
                    (string.IsNullOrWhiteSpace(nrOfWheels) || spot.ParkedVehicle.vehicleData.NrOfWheels == wheels) &&
                    (string.IsNullOrWhiteSpace(fuelType) || spot.ParkedVehicle.vehicleData.FuelType == (FuelType)fuel)
                    );
            return list;
        }

        public bool ParkVehicle(T parkingSpot, Vehicle vehicle, out string message)
        {
            if (parkingSpot.ParkVehicle(vehicle, out message))
            {
                return true;
            }
            else
            {
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
