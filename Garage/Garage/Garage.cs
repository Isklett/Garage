
using System.Collections;

namespace Garage.Garage
{
    internal class Garage<T> : IEnumerable<T> where T : Floor
    {
        private T[] _floors;

        public Garage(int nrOfFloors)
        {
            _floors = new T[nrOfFloors];
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_floors).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
