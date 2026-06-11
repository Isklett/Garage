using Garage.Interfaces;
using Garage.UI;
using Garage.Vehicles;

namespace Garage.Garage
{
    internal class GarageHandler
    {
        enum MenuState
        {
            MainMenu,
            GarageMenu,
            ParkVehicleMenu,
            RemoveVehicleMenu,
            FindVehicleMenu,
            ListParkedVehiclesMenu
        }
        private Garage<ParkingSpot> _garage { get; set; }
        private readonly IConsoleUI _ui;
        // Reference to the currently active menu
        private Dictionary<ConsoleKey, Action> _currentMenu;
        private readonly Dictionary<ConsoleKey, Action> _mainMenu;
        private readonly Dictionary<ConsoleKey, Action> _garageMenu;
        MenuState _currentMenuState = MenuState.MainMenu;
        private readonly Dictionary<MenuState, string[]> _menuOptions;
        private bool _appRunning { get; set; }

        public GarageHandler(IConsoleUI ui)
        {
            _ui = ui;
            _garage = new Garage<ParkingSpot>(100, _ui);
            _mainMenu = new Dictionary<ConsoleKey, Action>()
            {
                { ConsoleKey.D1, ChooseGarage},
                { ConsoleKey.D2, ListParkedVehicles},
                { ConsoleKey.D0, Exit }
            };
            _garageMenu = new Dictionary<ConsoleKey, Action>()
            {
                //{ ConsoleKey.D1, ParkVehicle},
                //{ ConsoleKey.D2, RemoveVehicle},
                { ConsoleKey.Escape, () => GoToMenu(MenuState.MainMenu) },
            };

            _currentMenu = _mainMenu;

            _menuOptions = new Dictionary<MenuState, string[]>()
            {
                { MenuState.MainMenu, new string[] { "1. Choose Garage", "2. List Parked Vehicles", "3. Find Vehicle", "0. Exit" }},
                { MenuState.GarageMenu, new string[] { "1. Park Vehicle", "2. Remove Vehicle", "Escape. Main menu" }},
                { MenuState.ParkVehicleMenu, new string[] { "1. Car", "2. Motorcycle", "3. Truck", "4. Bus", "5. Airplane", "Escape. Main menu" } },
                { MenuState.RemoveVehicleMenu, new string[] { "1. By Parking Spot", "2. By Registration Number", "Escape. Main menu" } },
                { MenuState.FindVehicleMenu, new string[] { "1. By Registration Number", "2. By Parking Spot", "Escape. Main menu" } },
                { MenuState.ListParkedVehiclesMenu, new string[] { "1. List All Parked Vehicles", "2. List Parked Vehicles by Type", "Escape. Main menu" }}
            };
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
                _ui.Draw(_menuOptions.GetValueOrDefault(_currentMenuState), _garage.GetOccupiedSpotList());

                //GetCommand
                GetCommand();
                //Act

                //DrawMap

                //EnemyAction

                //DrawMap


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

        private bool RemoveVehicle(ParkingSpot parkingSpot, Vehicle vehicle)
        {
            if (vehicle != null)
            {
                return _garage.RemoveVehicle(parkingSpot, vehicle);
            }
            else
            {
                return _garage.RemoveVehicle(parkingSpot);
            }
        }

        private void GoToMenu(MenuState menu)
        {
            _currentMenuState = menu;
            switch (menu)
            {
                case MenuState.MainMenu:
                    _currentMenu = _mainMenu;
                    break;
                case MenuState.GarageMenu:
                    _currentMenu = _garageMenu;
                    break;
                case MenuState.ParkVehicleMenu:
                    // Set current menu to ParkVehicleMenu
                    break;
                case MenuState.RemoveVehicleMenu:
                    // Set current menu to RemoveVehicleMenu
                    break;
                case MenuState.FindVehicleMenu:
                    // Set current menu to FindVehicleMenu
                    break;
                case MenuState.ListParkedVehiclesMenu:
                    // Set current menu to ListParkedVehiclesMenu
                    break;
            }
            _ui.Draw(_menuOptions.GetValueOrDefault(_currentMenuState), _garage.GetOccupiedSpotList());
        }

        private bool ParkVehicle()
        {
            throw new NotImplementedException();
        }

        private Vehicle FindVehicle()
        {
            throw new NotImplementedException();
        }

        private void ListParkedVehicles()
        {
            throw new NotImplementedException();
        }

        private void ChooseGarage()
        {
            GoToMenu(MenuState.GarageMenu);
        }
        private void Exit()
        {
            _appRunning = false;
        }
    }
}
