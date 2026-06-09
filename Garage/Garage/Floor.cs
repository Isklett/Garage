

namespace Garage.Garage
{
    internal class Floor
    {
        public int Level { get; }

        private readonly List<ParkingSpot> _spots = new();
        public IReadOnlyList<ParkingSpot> Spots => _spots;

        public Floor(int level, List<ParkingSpot>? initial = null)
        {
            Level = level;
            if (initial != null) _spots.AddRange(initial);
        }

        public void AddSpot(ParkingSpot spot) => _spots.Add(spot);
        public bool RemoveSpot(int spotId)
        {
            var idx = _spots.FindIndex(s => s.SpotNumber == spotId);
            if (idx >= 0) { _spots.RemoveAt(idx); return true; }
            return false;
        }


    }
}
