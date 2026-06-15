
namespace Garage.ValueTypes
{
    public static class SearchParameters
    {

        public record struct VehicleSearch
            (
            string RegNr,
            string Make,
            string Model,
            string Color,
            string NrOfWheels,
            string FuelType
            );
    }
}
