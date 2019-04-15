using ElevatorProject.Enums;
using ElevatorProject.Models.Interfaces;
using ElevatorProject.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorProject.Models
{
    public class Elevator : IElevator
    {
        #region vars and constructor
        private readonly int _bottomFloor;
        private readonly int _id;
        private readonly double _maxWeight;
        private readonly int _topFloor;

        private int currentFloor { get; set; }
        private DoorStatus doorStatus { get; set; }
        private ElevatorOperationStatus operationStatus { get; set; }

        public Elevator(int id, int bottomFloor, int topFloor, int startFloor, double maxWeight)
        {
            _id = id;
            _bottomFloor = bottomFloor;
            _topFloor = topFloor;
            _maxWeight = maxWeight;

            currentFloor = startFloor;
            operationStatus = ElevatorOperationStatus.AVAILABLE;
        }
        #endregion

        public bool CloseDoors()
        {            
            doorStatus = DoorStatus.CLOSED;

            Console.WriteLine($"Elevator {_id} doors closed");

            return false;
        }

        public int GetBottomFloor()
        {
            return _bottomFloor;
        }

        public int GetCurrentFloor()
        {
            return currentFloor;
        }

        public int GetId()
        {
            return _id;
        }

        public ElevatorOperationStatus GetOperationalState()
        {
            return operationStatus;
        }

        public int GetTopFloor()
        {
            return _topFloor;
        }

        public async Task MoveBetweenFloors()
        {
            await Task.Delay(1000);

            currentFloor = operationStatus == ElevatorOperationStatus.MOVING_UP
                ? currentFloor += 1
                : currentFloor -= 1;

            Console.WriteLine($"Elevator {_id} status: {operationStatus} to Floor: {currentFloor}");
        }

        public bool OpenDoors()
        {
            doorStatus = DoorStatus.OPEN;

            Console.WriteLine($"Elevator {_id} doors open. People get out... People get in...");

            return true;
        }

        public void ReadyElevatorForJourney(Request request)
        {
            //If the doors are open close them after all safety checks complete
            if (doorStatus == DoorStatus.OPEN)
            {
                while (!RunSafetyChecks())
                {
                    //Wait 1 second before re-trying
                    Thread.Sleep(1000);
                }

                CloseDoors();
            }

            //Check if we are travelling up or down and change the staus from 'Available' to 'Moving...'
            operationStatus = request.Floor > currentFloor
            ? ElevatorOperationStatus.MOVING_UP
            : ElevatorOperationStatus.MOVING_DOWN;

            Console.WriteLine($"Elevator {_id} beginning journey from Floor: {currentFloor}");
        }

        public void ReadyElevatorForNewPassengers()
        {
            OpenDoors();

            SetElevatorAsAvailable();
        }

        public void RunEnroutePickupRoutine(Request request)
        {
            OpenDoors();

            //Give people 1 second to board before weighing and closing the Doors
            Task.Delay(1000);

            ReadyElevatorForJourney(request);
        }

        public void SetElevatorAsAvailable()
        {
            operationStatus = ElevatorOperationStatus.AVAILABLE;

            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Elevator {_id} status: {operationStatus}");
        }

        public void SetToMaintenanceMode()
        {
            operationStatus = ElevatorOperationStatus.MAINTENANCE_MODE;
        }

        public bool RunSafetyChecks()
        {
            if (!WeighElevator())
            {
                operationStatus = ElevatorOperationStatus.OVERWEIGHT;

                Console.WriteLine($"Elevator {_id} status: {operationStatus}. Max weight limit: {_maxWeight}. People leave..");

                return false;
            }
            
            Console.WriteLine($"Elevator {_id} weigh in successful");

            return true;
        }

        public bool WeighElevator()
        {
            //10% chance of simulating an overweigh (for demo purposes)
            return new Random().Next(0, 10) != 1;
        }
    }
}
