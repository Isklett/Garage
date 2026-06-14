namespace Garage.Interfaces
{
    public interface IParkable
    {
        DateTime ArrivalTime { get; set; }

        bool ParkVehicle();
    }
}
