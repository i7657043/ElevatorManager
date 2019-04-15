using ElevatorProject.Enums;
using ElevatorProject.Extensions;
using ElevatorProject.Interfaces;
using ElevatorProject.Models;
using ElevatorProject.Models.Interfaces;
using ElevatorProject.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorProject.Implementations
{
    public class ElevatorManager : IElevatorManager
    {
        #region vars and constructor
        private readonly IElevatorService _elevatorService;

        private List<IElevator> controlledElevators { get; set; }

        //Could have used a Queue as the List is more or less used like a queue
        //but the ability to inspect all requests when an Elevator is passing floors (and remove the element at any index on the fly) was wanted
        private List<Request> requestList { get; set; }
        
        
        public ElevatorManager(IElevatorService elevatorService)
        {
            _elevatorService = elevatorService;
        }
        #endregion

        public void MakeElevatorManagerRequest(Request request)
        {
            if (!requestList.Any(x => x.Floor == request.Floor) && controlledElevators.ElevatorsOperateOnThisFloor(request.Floor))
            {
                requestList.Add(request);

                Console.ForegroundColor = ConsoleColor.Green;
                string panelRequestString = request.ElevatorId != 0 ? $". Elevator Id: {request.ElevatorId}" : null;
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {request.RequestType} Request added for Floor {request.Floor}{panelRequestString}\n");
                Console.ForegroundColor = ConsoleColor.White;

                return;
            }

            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Invalid request: {request.RequestType} Request for Floor {request.Floor}. Ignored\n");
        }

        public async Task<int> RunElevatorManager()
        {
            if (!ElevatorManagerInitialisation())
            {
                return -1;
            }

            PrintControlledElevatorState();

            PrintRemainingRequests(requestList);

            //The ElevatorManager never sleeps, it checks for a new Request to pick up at set intervals
            while (true)
            {
                try
                {
                    if (requestList.Count == 0)
                    {
                        //Wait before checking the Request list again
                        Thread.Sleep(1000);
                        continue;
                    }

                    Request floorRequest = requestList[0];

                    IElevator elevator = ChooseElevatorDependingOnRequestType(floorRequest);
                    if (elevator == null)
                    {
                        //Wait 1 second before checking for an available Elevator again
                        Thread.Sleep(1000);
                        continue;
                    }

                    //An Elevator was found to service the Request, it can be removed
                    requestList.RemoveAt(0);                                      
                  
                    await MakeElevatorJourney(floorRequest, elevator);

                    PrintControlledElevatorState();

                    PrintRemainingRequests(requestList);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private IElevator ChooseElevatorDependingOnRequestType(Request floorRequest)
        {
            //If a panel request comes in from Elevator panel then that Elevator` must obviosuly go on the journey. If its a Wall request the best possible availble Elevator can pick it up
            return floorRequest.RequestType == RequestType.PANEL
                ? controlledElevators.FirstOrDefault(x => x.GetId() == floorRequest.ElevatorId)
                : controlledElevators.ChooseAppropriateAvailableElevator(floorRequest.Floor);
        }

        private async Task RunJourney(Request request, IElevator elevator)
        {
            //Move the Elevator in the correct direction until it reaches the request floor
            while (elevator.GetCurrentFloor() != request.Floor)
            {
                await elevator.MoveBetweenFloors();

                //If we come across any Floors that have requests in our Direction (among other conditions) then stop to pick them up and remove the Request
                if (ElevatorShouldMakeEnroutePickup(elevator))
                {
                    Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Elevator stopped to make {requestList.FirstOrDefault(x => x.Floor == elevator.GetCurrentFloor()).RequestType} request pickup " +
                        $"on Floor: {elevator.GetCurrentFloor()} en-route to Floor: {request.Floor}");

                    elevator.RunEnroutePickupRoutine(request);

                    requestList.RemoveAll(x => x.Floor == elevator.GetCurrentFloor());
                }

                //Elevator cant travel in this direction any further
                if (elevator.IsAtFloorRangeLimit())
                {
                    //break;
                }
            }

            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Elevator {elevator.GetId()} completed {request.RequestType} request for Floor: {elevator.GetCurrentFloor()}");
        }

        private bool ElevatorManagerInitialisation()
        {
            try
            {
                controlledElevators = _elevatorService.GetElevators();

                requestList = new List<Request>();

                return true;
            }
            catch (Exception)
            {
                //Log error or rasie Error event
                Console.WriteLine("Initialisation Error\n\nPress any key to Exit..\n");

                return false;
            }
        }

        private bool ElevatorShouldMakeEnroutePickup(IElevator elevator)
        {
            return requestList.Any(x => x.Floor == elevator.GetCurrentFloor()
                                && (x.RequestType == RequestType.WALL || x.ElevatorId == elevator.GetId()));
        }

        private async Task MakeElevatorJourney(Request request, IElevator elevator)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("-----Journey Cycle Started-----");
            Console.ForegroundColor = ConsoleColor.White;

            try
            {
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Elevator {elevator.GetId()} responding to {request.RequestType} request for floor {request.Floor}");

                //Request was on the current floor
                if (elevator.GetCurrentFloor() == request.Floor)
                {
                    elevator.ReadyElevatorForNewPassengers();

                    return;
                }

                elevator.ReadyElevatorForJourney(request);
                
                await RunJourney(request, elevator);

                elevator.ReadyElevatorForNewPassengers();
            }
            catch (Exception ex) 
            {
                //Obviously log this error in reality
                Console.WriteLine(ex.Message);

                elevator.SetToMaintenanceMode();
                _elevatorService.RaiseElevatorIssueAlert(elevator.GetId());

                throw ex;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("-----Journey Cycle Complete-----");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void PrintControlledElevatorState()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n------Elevator Information-------");
            Console.WriteLine();
            foreach (Elevator elevator in controlledElevators)
            {
                Console.WriteLine($"Elevator Id: {elevator.GetId()}");
                Console.WriteLine($"Current Floor: {elevator.GetCurrentFloor()}");
                Console.WriteLine("");
            }
            Console.WriteLine("----------------------------------");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void PrintRemainingRequests(List<Request> elevatorRequests)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (elevatorRequests.Count==0)
            {
                Console.WriteLine("\nNo Requests. Elevator-Manager waiting...\n");
                return;
            }
            
            Console.WriteLine("\n------Remaining Requests-------\n");
            foreach (Request request in elevatorRequests)
            {
                string panelRequestString = request.ElevatorId != 0 ? $"Elevator Id: {request.ElevatorId}" : null;
                Console.WriteLine($"Floor: {request.Floor} Type: {request.RequestType} {panelRequestString}");
            }
            Console.WriteLine("\n----------------------------------");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }
    }
}
