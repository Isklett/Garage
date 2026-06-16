using Garage.Interfaces;
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
        private bool _appRunning { get; set; } = false;

        public GarageHandler(IConsoleUI ui)
        {
            _ui = ui;
            var gar = new Garage<ParkingSpot<Vehicle>>("garage", 100, new Dimensions(5, 2.5, 10), _ui);
            _garages.Add(gar);
            _garage = _garages[0];

            PopulateMockGarage(_garage);

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

                _textToDraw.Clear();

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
            _ui.ClearScreen(1);
            SearchParameters.VehicleSearch searchParams = _ui.GetMultipleLines(_searchParameters, "Search");
            IEnumerable<ParkingSpot<Vehicle>> parkingSpots = _garage.SearchVehicles(searchParams.RegNr, searchParams.Make, searchParams.Model, searchParams.Color, searchParams.NrOfWheels, searchParams.FuelType);
            foreach (ParkingSpot<Vehicle> parkingSpot in parkingSpots)
            {
                if(parkingSpot.ParkedVehicle != null)
                {
                    string regNr = parkingSpot.ParkedVehicle.vehicleData.RegNr.ToString();
                    string make = parkingSpot.ParkedVehicle.vehicleData.Make.ToString();
                    string model = parkingSpot.ParkedVehicle.vehicleData.Model.ToString();
                    string color = parkingSpot.ParkedVehicle.vehicleData.Color.ToString();
                    string nrOfWheels = parkingSpot.ParkedVehicle.vehicleData.NrOfWheels.ToString();
                    string fuelType = parkingSpot.ParkedVehicle.vehicleData.FuelType.ToString();
                    _textToDraw.Add($"Parking spot {parkingSpot.SpotNumber}: {regNr} - {make} - {model} - {color} paint - {nrOfWheels} wheels - {fuelType}");
                }
            }
        }

        private void ListParkedVehicles()
        {
            _ui.ClearScreen(1);
            var occupiedSpots = _garage.GetOccupiedSpots();
            foreach (var spot in occupiedSpots)
            {
                _textToDraw.Add($"Spot {spot.SpotNumber}: {spot.ParkedVehicle?.vehicleData.Make} {spot.ParkedVehicle?.vehicleData.Model} ({spot.ParkedVehicle?.GetType().Name})");
            }
        }

        private void StartRemoveVehicle()
        {
            _ui.ClearScreen(1);
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
            _ui.ClearScreen(1);
            int? vehicleClassChoice = _ui.GetChoiceInput("What type of vehicle would you like to park? Make your desired choice:", VehicleClasses.Select(t => t.Name).ToList(), "No vehicles to choose from.");
            if (!vehicleClassChoice.HasValue)
            {
                _textToDraw.Add("Can't park, no vehicle types found.");
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
                _textToDraw.Add("No garages found.");
                return;
            }
            _garage = _garages[(int)garageChoice];
            GoToMenu(MenuState.GarageMenu);
        }
        private void Exit()
        {
            _appRunning = false;
        }

        private void PopulateMockGarage(Garage<ParkingSpot<Vehicle>> garage)
        {
            #region MockData
            var mockVehicles = new List<Vehicle>()
            {
                new Car("Toyota", "Corolla", "Blue", 4, 1.8f, Enumerators.VehicleEnums.FuelType.Gasoline, "REG-001", new Dimensions(4.5,1.8,1.4), 4),
                new Motorcycle("Yamaha", "MT-07", "Black", 2, 0.689f, Enumerators.VehicleEnums.FuelType.Gasoline, "REG-002", new Dimensions(2.1,0.8,1.1), 690),
                new Bus("Volvo", "7700", "White", 6, 5.0f, Enumerators.VehicleEnums.FuelType.Diesel, "REG-003", new Dimensions(12.0,2.5,3.2), 50),
                new Airplane("Cessna", "172", "White", 3, 180.0f, Enumerators.VehicleEnums.FuelType.Diesel, "REG-004", new Dimensions(8.28,11.0,2.7), 11.0),
                new Boat("Bayliner", "Element", "White", 0, 1.2f, Enumerators.VehicleEnums.FuelType.Gasoline, "REG-005", new Dimensions(6.5,2.5,2.5), Boat.MaterialTypes.Fiberglass),
                new Car("Honda", "Civic", "Red", 4, 2.0f, Enumerators.VehicleEnums.FuelType.Gasoline, "REG-006", new Dimensions(4.6,1.8,1.4), 4),
                new Motorcycle("Ducati", "Monster", "Red", 2, 1.26f, Enumerators.VehicleEnums.FuelType.Gasoline, "REG-007", new Dimensions(2.15,0.9,1.1), 1200),
                new Bus("Mercedes", "Citaro", "Yellow", 6, 6.0f, Enumerators.VehicleEnums.FuelType.Diesel, "REG-008", new Dimensions(10.5,2.5,3.0), 40),
                new Car("Tesla", "Model 3", "White", 4, 0.0f, Enumerators.VehicleEnums.FuelType.Electric, "REG-009", new Dimensions(4.7,1.85,1.44), 4),
                new Boat("Sea Ray", "SPX190", "Blue", 0, 1.0f, Enumerators.VehicleEnums.FuelType.Gasoline, "REG-010", new Dimensions(5.6,2.2,1.6), Boat.MaterialTypes.Aluminium),

                // Same model, different color/reg - Corolla repeats
                new Car("Toyota", "Corolla", "Red", 4, 1.8f, Enumerators.VehicleEnums.FuelType.Gasoline, "REG-011", new Dimensions(4.5,1.8,1.4), 4),
                new Car("Toyota", "Corolla", "Black", 4, 1.8f, Enumerators.VehicleEnums.FuelType.Hybrid, "REG-012", new Dimensions(4.5,1.8,1.4), 4),

                // Same brand, different model - Toyota repeats
                new Car("Toyota", "Camry", "Silver", 4, 2.5f, Enumerators.VehicleEnums.FuelType.Gasoline, "REG-013", new Dimensions(4.9,1.85,1.45), 4),
                new Car("Toyota", "RAV4", "Blue", 4, 2.5f, Enumerators.VehicleEnums.FuelType.Hybrid, "REG-014", new Dimensions(4.6,1.85,1.69), 4),

                // Same model - Civic repeats
                new Car("Honda", "Civic", "Black", 4, 2.0f, Enumerators.VehicleEnums.FuelType.Gasoline, "REG-015", new Dimensions(4.6,1.8,1.4), 4),

                // Same brand - Honda repeats
                new Car("Honda", "Accord", "Gray", 4, 2.0f, Enumerators.VehicleEnums.FuelType.Hybrid, "REG-016", new Dimensions(4.9,1.86,1.45), 4),

                // Same model - Model 3 repeats
                new Car("Tesla", "Model 3", "Black", 4, 0.0f, Enumerators.VehicleEnums.FuelType.Electric, "REG-017", new Dimensions(4.7,1.85,1.44), 4),

                // Same brand - Tesla repeats
                new Car("Tesla", "Model Y", "White", 4, 0.0f, Enumerators.VehicleEnums.FuelType.Electric, "REG-018", new Dimensions(4.75,1.92,1.62), 4),

                // Same model - MT-07 repeats
                new Motorcycle("Yamaha", "MT-07", "Blue", 2, 0.689f, Enumerators.VehicleEnums.FuelType.Gasoline, "REG-019", new Dimensions(2.1,0.8,1.1), 690),

                // Same brand - Yamaha repeats
                new Motorcycle("Yamaha", "R6", "Black", 2, 0.599f, Enumerators.VehicleEnums.FuelType.Gasoline, "REG-020", new Dimensions(2.04,0.7,1.13), 600),

                // Same brand - Ducati repeats
                new Motorcycle("Ducati", "Panigale V4", "Red", 2, 1.1f, Enumerators.VehicleEnums.FuelType.Gasoline, "REG-021", new Dimensions(2.04,0.75,1.13), 1100),

                // Same brand - Volvo repeats
                new Bus("Volvo", "9700", "Blue", 6, 5.0f, Enumerators.VehicleEnums.FuelType.Diesel, "REG-022", new Dimensions(12.5,2.55,3.4), 55),

                // Same brand - Mercedes repeats
                new Bus("Mercedes", "Sprinter", "White", 4, 2.1f, Enumerators.VehicleEnums.FuelType.Diesel, "REG-023", new Dimensions(7.3,2.0,2.7), 16),

                // Same model - 172 repeats
                new Airplane("Cessna", "172", "Blue", 3, 180.0f, Enumerators.VehicleEnums.FuelType.Gasoline, "REG-024", new Dimensions(8.28,11.0,2.7), 11.0),

                // Same brand - Cessna repeats
                new Airplane("Cessna", "182", "White", 3, 230.0f, Enumerators.VehicleEnums.FuelType.Gasoline, "REG-025", new Dimensions(8.84,11.0,2.8), 11.0),

                // Same brand - Bayliner repeats
                new Boat("Bayliner", "VR5", "Black", 0, 1.5f, Enumerators.VehicleEnums.FuelType.Gasoline, "REG-026", new Dimensions(6.1,2.4,2.4), Boat.MaterialTypes.Fiberglass),

                // Same brand - Sea Ray repeats
                new Boat("Sea Ray", "SLX 280", "White", 0, 1.8f, Enumerators.VehicleEnums.FuelType.Gasoline, "REG-027", new Dimensions(8.6,2.6,2.6), Boat.MaterialTypes.Fiberglass),

                // New brand - BMW
                new Car("BMW", "3 Series", "Gray", 4, 2.0f, Enumerators.VehicleEnums.FuelType.Diesel, "REG-028", new Dimensions(4.7,1.83,1.44), 4),

                // Same brand - BMW repeats
                new Car("BMW", "X5", "Black", 4, 3.0f, Enumerators.VehicleEnums.FuelType.Diesel, "REG-029", new Dimensions(4.92,2.0,1.75), 4),

                // New brand - Kawasaki
                new Motorcycle("Kawasaki", "Ninja 650", "Green", 2, 0.649f, Enumerators.VehicleEnums.FuelType.Gasoline, "REG-030", new Dimensions(2.1,0.74,1.13), 650),
            };
            #endregion

            int parkedCount = 0;
            foreach (var vehicle in mockVehicles)
            {
                var freeSpots = garage.GetFreeSpots(vehicle.vehicleData.Dimensions);
                if (freeSpots.Count > 0)
                {
                    var spot = freeSpots.First();
                    if (garage.ParkVehicle(spot, vehicle))
                    {
                        //_textToDraw.Add($"[Mock] Parked {vehicle.vehicleData.Make} {vehicle.vehicleData.Model} ({vehicle.GetType().Name}) at spot {spot.SpotNumber}");
                        parkedCount++;
                    }
                }
                else
                {
                    //_textToDraw.Add($"[Mock] No fitting spot for {vehicle.vehicleData.Make} {vehicle.vehicleData.Model} ({vehicle.GetType().Name})");
                }
            }

            _textToDraw.Add($"PopulateMockGarage: attempted {mockVehicles.Count} vehicles, parked {parkedCount}.");
        }
    }
}
