using System;
using System.Collections.Generic;
using System.Text;

namespace Garage.Interfaces
{
    public interface ILimitedList<T> : IEnumerable<T>
    {
        T this[int index] { get; }

        int Count { get; }
        bool IsFull { get; }
    }
}
