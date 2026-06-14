using Garage.Interfaces;
using Garage.UI;
using Garage.ValueTypes;
using Garage.Vehicles;
using Garage.Vehicles.VehicleTypes;

namespace Garage.Garage
{
    internal class GarageHandler
    {
        enum MenuState
        {
            MainMenu,
            Custom,
            GarageMenu
        }
        private List<Garage<ParkingSpot<Vehicle>>> _garages = new List<Garage<ParkingSpot<Vehicle>>>();
        private Garage<ParkingSpot<Vehicle>> _garage { get; set; }
        private readonly IConsoleUI _ui;
        // Reference to the currently active menu
        private Dictionary<ConsoleKey, Action> _currentMenu;
        private readonly Dictionary<ConsoleKey, Action> _mainMenu;
        private readonly Dictionary<ConsoleKey, Action> _customMenu;
        private readonly Dictionary<ConsoleKey, Action> _garageMenu;
        MenuState _currentMenuState = MenuState.MainMenu;
        private readonly Dictionary<MenuState, string[]> _menuOptions;

        private readonly string[] _searchParameters = { "RegNr", "Make", "Model", "Color", "Number Of Wheels", "Fuel Type" }; 

        private List<string> _textToDraw = new List<string>();

        public List<Type> VehicleClasses;
        private bool _appRunning { get; set; }

        public GarageHandler(IConsoleUI ui)
        {
            _ui = ui;
            var gar = new Garage<ParkingSpot<Vehicle>>("garage", 100, new Dimensions(5, 2.5, 10), _ui);
            _garages.Add(gar);
            _garage = _garages[0];
            _mainMenu = new Dictionary<ConsoleKey, Action>()
            { 
                { ConsoleKey.D1, AddGarage},
                { ConsoleKey.D2, ChooseGarage},
                { ConsoleKey.D0, Exit }
            };

            _customMenu = new Dictionary<ConsoleKey, Action>()
            {
                { ConsoleKey.Escape, () => GoToMenu(MenuState.MainMenu)}
            };

            _garageMenu = new Dictionary<ConsoleKey, Action>()
            {
                { ConsoleKey.D1, StartParkVehicle},
                { ConsoleKey.D2, StartRemoveVehicle},
                { ConsoleKey.D3, ListParkedVehicles},
                { ConsoleKey.D4, StartFindVehicle },
                { ConsoleKey.Escape, () => GoToMenu(MenuState.MainMenu) },
            };

            _currentMenu = _mainMenu;

            _menuOptions = new Dictionary<MenuState, string[]>()
            {
                { MenuState.MainMenu, new string[] { "1. Add Garage", "2. Choose Garage", "0. Exit" }},
                { MenuState.Custom, new string[] { " Escape. Main Menu" } },
                { MenuState.GarageMenu, new string[] { "1. Park Vehicle", "2. Remove Vehicle", "3. List Parked Vehicles", "4. Find vehicle", "Escape. Main menu" }}
            };

            VehicleClasses = typeof(Vehicle)
                .Assembly
                .GetTypes()
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    typeof(Vehicle).IsAssignableFrom(t))
                .ToList();
        }

        internal void Run()
        {
            Play();
        }

        private void Play()
        {
            _appRunning = true;
            do
            {
                //DrawMap
                _ui.Draw(_menuOptions.GetValueOrDefault(_currentMenuState), _textToDraw);

                //GetCommand
                GetCommand();


            } while (_appRunning);
        }
        private void GetCommand()
        {
            ConsoleKey keyPressed = _ui.GetKey();

            if (_currentMenu.ContainsKey(keyPressed))
            {
                _currentMenu[keyPressed]?.Invoke();
            }
        }

        private void GoToMenu(MenuState menu)
        {
            _currentMenuState = menu;
            _ui.ClearScreen(0);
            switch (menu)
            {
                case MenuState.MainMenu:
                    _currentMenu = _mainMenu;
                    break;
                case MenuState.Custom:
                    _currentMenu = _customMenu;
                    break;
                case MenuState.GarageMenu:
                    _currentMenu = _garageMenu;
                    break;
                default:
                    _currentMenu = _customMenu;
                    break;
            }
            var temp = _garage.GetOccupiedSpotList();
        }

        private void StartFindVehicle()
        {
            SearchParameters.VehicleSearch searchParams = _ui.GetMultipleLines(_searchParameters, "Search");
            _garage.SearchVehicles(searchParams.RegNr, searchParams.Make, searchParams.Model, searchParams.Color, searchParams.NrOfWheels, searchParams.FuelType);
        }

        private Vehicle? FindVehicle()
        {
            string vehicleRegNr = _ui.GetStringInput("Enter registration number of the vehicle to find:");
            if(_garage.TryFindVehicle(vehicleRegNr, out ParkingSpot<Vehicle>? parkingSpot))
            {
                _ui.ShowMessage($"Vehicle with registration number {vehicleRegNr} is parked at spot {parkingSpot.SpotNumber}.");
                return parkingSpot.ParkedVehicle;
            }
            else
            {
                _ui.ShowMessage($"Vehicle with registration number {vehicleRegNr} not found in the garage.");
                return null;
            }
        }

        private void ListParkedVehicles()
        {
            var occupiedSpots = _garage.GetOccupiedSpots();
            foreach (var spot in occupiedSpots)
            {
                _textToDraw.Add($"Spot {spot.SpotNumber}: {spot.ParkedVehicle?.vehicleData.Make} {spot.ParkedVehicle?.vehicleData.Model} ({spot.ParkedVehicle?.GetType().Name})");
            }
        }

        private void StartRemoveVehicle()
        {
            string vehicleRegNr = _ui.GetStringInput("Enter registration number of the vehicle to remove:");
            if (_garage.TryFindVehicle(vehicleRegNr, out ParkingSpot<Vehicle>? parkingSpot))
            {
                bool succeeded = _garage.RemoveVehicle(parkingSpot);
                if (succeeded)
                {
                    _ui.ShowMessage($"Vehicle with registration number {vehicleRegNr} removed from parking spot {parkingSpot.SpotNumber}.");

                }
                else
                {
                    _ui.ShowMessage($"Failed to remove vehicle with registration number {vehicleRegNr} from parking spot {parkingSpot.SpotNumber}.");
                }
            }
        }

        private void StartParkVehicle()
        {
            int? vehicleClassChoice = _ui.GetChoiceInput("What type of vehicle would you like to park? Make your desired choice:", VehicleClasses.Select(t => t.Name).ToList(), "No vehicles to choose from.");
            if (!vehicleClassChoice.HasValue)
            {
                Console.WriteLine("Can't park, no vehicle types found.");
                return;
            }
            Type type = VehicleClasses[(int)vehicleClassChoice];
            Vehicle vehicleToPark = null!;
            switch (type)
            {
                case Type t when t == typeof(Car):
                    Car car = Car.Create(_ui);
                    vehicleToPark = car;
                    break;
                case Type t when t == typeof(Motorcycle):
                    Motorcycle motorcycle = Motorcycle.Create(_ui);
                    vehicleToPark = motorcycle;
                    break;
                case Type t when t == typeof(Bus):
                    Bus bus = Bus.Create(_ui);
                    vehicleToPark = bus;
                    break;
                case Type t when t == typeof(Airplane):
                    Airplane aiplane = Airplane.Create(_ui);
                    vehicleToPark = aiplane;
                    break;
                case Type t when t == typeof(Boat):
                    Boat boat = Boat.Create(_ui);
                    vehicleToPark = boat;
                    break;
                default:
                    _ui.ShowError("Type is not a vehicle.");
                    break;
            }

            int? spotNr = _ui.GetChoiceInput("Here are your available parking spots. Make your desired choice:", _garage.GetFreeSpots(vehicleToPark.vehicleData.Dimensions).Select(t => t.SpotNumber.ToString()).ToList(), "There are no free spots that fit your vehicle.");
            if(spotNr.HasValue)
            {
                ParkingSpot<Vehicle> spot = _garage[(int)spotNr];
                _garage.ParkVehicle(spot, vehicleToPark);
            }
            
        }

        private void AddGarage()
        {
            string name = _ui.GetStringInput("Enter new garage name:");
            int capacity = _ui.GetIntInput("Enter garage capacity:");
            _garage = new Garage<ParkingSpot<Vehicle>>(name, capacity, new Dimensions(5, 2.5, 10), _ui);
            _garages.Add(_garage);
        }
        private void ChooseGarage()
        {
            int? garageChoice = _ui.GetChoiceInput("Available garages. Make your desired choice:", _garages.Select(t => t.Name).ToList(), "No garages to choose.");
            if (!garageChoice.HasValue)
            {
                Console.WriteLine("No garages found.");
                return;
            }
            _garage = _garages[(int)garageChoice];
            GoToMenu(MenuState.GarageMenu);
        }
        private void Exit()
        {
            _appRunning = false;
        }
    }
}
