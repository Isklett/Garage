
using Garage.Garage;
using Garage.UI;

namespace Garage
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var garageHandler = new GarageHandler(new ConsoleUI());

            garageHandler.Run();

            Console.WriteLine("See you next time!");
        }
    }
}
