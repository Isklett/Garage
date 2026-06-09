using System;
using System.Collections.Generic;
using System.Text;

namespace Garage.ValueTypes
{
    public readonly struct Dimensions
    {
        public double Length { get; init; }
        public double Width { get; init; }

        public Dimensions(double length, double width)
        {
            Length = length;
            Width = width;
        }
    }
}
