using ElevatorProject.Enums;
using ElevatorProject.Interfaces;
using ElevatorProject.Models;
using ElevatorProject.Services.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorProject.Services.Implementations
{
    public class ElevatorManagerTestSimulatorService : IElevatorManagerTestSimulatorService
    {
        #region vars and constructor
        private readonly IElevatorManager _manager;

        public ElevatorManagerTestSimulatorService(IElevatorManager manager)
        {
            _manager = manager;
        }
        #endregion

        public void RunAutomatedSimulation()
        {
            Random randomRequest = new Random();

            //Generate random requests at every key press until Escape is pressed
            do
            {
                for (int i = 0; i < 5; i++)
                {
                    int randomRequestType = randomRequest.Next(1, 3);

                    _manager.MakeElevatorManagerRequest(new Request
                    {
                        Floor = randomRequest.Next(-3, 21),
                        RequestType = (RequestType)randomRequestType,
                        ElevatorId = randomRequestType == 2 //If generating a Panel request then generate a random Elevator Id to go with it
                        ? randomRequest.Next(1, 3)
                        : 0
                    });
                }

            } while (Console.ReadKey().Key != ConsoleKey.Escape);
        }

        public void RunManualSimulation()
        {
            //Simulate triggering requests, the Elevator manager allocate Elevators to pick them up. Elevator defaults set in ElevatorService.GetElevators() (Next Bookmark)
            //Modify any requests for Testing as neccessary
            _manager.MakeElevatorManagerRequest(new Request { Floor = 2, RequestType = RequestType.WALL });
            _manager.MakeElevatorManagerRequest(new Request { Floor = 12, RequestType = RequestType.WALL });
            _manager.MakeElevatorManagerRequest(new Request { Floor = 7, RequestType = RequestType.PANEL, ElevatorId = 1, });
            _manager.MakeElevatorManagerRequest(new Request { Floor = 4, RequestType = RequestType.WALL });


            _manager.MakeElevatorManagerRequest(new Request { Floor = 1, RequestType = RequestType.WALL });
            //Simulates 2 people getting into Elevatpr 1 and pressing Floor 3 and 5 buttons on panel
            _manager.MakeElevatorManagerRequest(new Request { Floor = 3, RequestType = RequestType.PANEL, ElevatorId = 2, });
            _manager.MakeElevatorManagerRequest(new Request { Floor = 5, RequestType = RequestType.PANEL, ElevatorId = 2, });
        }
    }
}
