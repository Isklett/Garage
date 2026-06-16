using Garage.Interfaces;
using Garage.ValueTypes;
using Garage.Vehicles;
using System.Diagnostics.CodeAnalysis;

namespace Garage.Garage
{
    internal class ParkingSpot<T> where T : Vehicle
    {
        public Dimensions ParkingSize { get; init; }
        public int SpotNumber { get; init; }

        [MemberNotNullWhen(true, nameof(ParkedVehicle))] // Indicates that ParkedVehicle is not null when IsOccupied is true
        public bool IsOccupied => ParkedVehicle != null;
        public Vehicle? ParkedVehicle { get; private set; }

        public ParkingSpot(double length, double width, double height, int spotNumber)
        {
            ParkingSize = new Dimensions(length, width, height);
            SpotNumber = spotNumber;
        }

        public bool ParkVehicle(T vehicle, out string message)
        {
            if(vehicle is IParkable)
            {
                if (ParkedVehicle != null)
                {
                    message = $"Parking spot {SpotNumber} is already occupied by {ParkedVehicle.vehicleData.VehicleType} - {ParkedVehicle.vehicleData.Make} {ParkedVehicle.vehicleData.Model}.";
                    return false;
                }
                else if (vehicle.vehicleData.Dimensions.Length > ParkingSize.Length || vehicle.vehicleData.Dimensions.Width > ParkingSize.Width || vehicle.vehicleData.Dimensions.Height > ParkingSize.Height)
                {
                    message = $"{vehicle.vehicleData.VehicleType} - {vehicle.vehicleData.Make} {vehicle.vehicleData.Model} does not fit in parking spot {SpotNumber}.";
                    return false;
                }
                else
                {
                    ParkedVehicle = vehicle;
                    message = $"{vehicle.vehicleData.VehicleType} - {vehicle.vehicleData.Make} {vehicle.vehicleData.Model} has been parked in parking spot {SpotNumber}.";
                    return true;
                }
            }
            else
            {
                message = "Vehicle does not implement IParkable interface and cannot be parked.";
                return false;
            }
        }

        public bool RemoveVehicle()
        {
            if (ParkedVehicle == null)
            {
                Console.WriteLine($"Parking spot {SpotNumber} is already empty.");
                return false;
            }
            else
            {
                Console.WriteLine($"Vehicle {ParkedVehicle.vehicleData.Make} {ParkedVehicle.vehicleData.Model} removed from spot {SpotNumber}.");
                ParkedVehicle = null;
                return true;
            }
        }
    }
}
