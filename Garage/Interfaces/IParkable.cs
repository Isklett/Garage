using Garage.ValueTypes;

namespace Garage.Interfaces
{
    public interface IParkable
    {
        string RegistrationNumber { get; }
        Dimensions Dimensions { get; }
        DateTime ArrivalTime { get; set; }

        bool ParkVehicle();
    }
}
