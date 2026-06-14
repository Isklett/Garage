namespace Garage.ValueTypes
{
    public readonly struct Dimensions
    {
        public double Length { get; init; }
        public double Width { get; init; }
        public double Height { get; init; }

        public bool CanFit(Dimensions dimensions)
        {
            if(dimensions.Length < Length && dimensions.Width < Width && dimensions.Height < Height)
                return true;
            else
                return false;
        }

        public Dimensions(double length, double width, double height)
        {
            Length = length;
            Width = width;
            Height = height;
        }
    }
}
