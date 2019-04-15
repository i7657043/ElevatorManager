using ElevatorProject.Enums;
using ElevatorProject.Models;
using ElevatorProject.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorProject.Extensions
{
    public static class ElevatorExtensions
    {
        public static bool ElevatorsOperateOnThisFloor(this List<IElevator> elevators, int floorRequest)
        {
            return elevators.Any(x => floorRequest >= x.GetBottomFloor() && floorRequest <= x.GetTopFloor());
        }

        public static bool IsAtFloorRangeLimit(this IElevator elevator)
        {
            return elevator.GetCurrentFloor() == elevator.GetTopFloor() || elevator.GetCurrentFloor() == elevator.GetBottomFloor();
        }

        public static IElevator ChooseAppropriateAvailableElevator(this List<IElevator> elevators, int requestedFloor)
        {
            IElevator availableElevator = elevators.FirstOrDefault(x => x.GetOperationalState() == ElevatorOperationStatus.AVAILABLE);
            if (availableElevator == null)
            {
                //All Elevators are busy or unavailable in some way (Maintenance mode, etc.), come back
                return null;
            }

            int currentSmallestDifference = GetDifferenceBetweenFloors(availableElevator.GetCurrentFloor(), requestedFloor);

            //Start loop from 1 as we are checking agasint first available Elevator
            foreach(IElevator elevator in elevators.Skip(1))
            {
                //Is the request coming from below this Elevators current floor
                if (elevator.GetCurrentFloor() > requestedFloor)
                {
                    //If another Elevator is currently travelling down then it will already pickup the request as it hits the floor
                    if (ElevatorIsOnACloserFloor(requestedFloor, elevator.GetCurrentFloor(), currentSmallestDifference)
                        && !elevators.Any(x => x.GetOperationalState() == ElevatorOperationStatus.MOVING_DOWN)) 
                    {
                        availableElevator = elevator; //Our new best selection of Elevator
                        currentSmallestDifference = elevator.GetCurrentFloor() - requestedFloor;
                    }
                }
                else
                {
                    if (ElevatorIsOnACloserFloor(elevator.GetCurrentFloor(), requestedFloor, currentSmallestDifference)
                    && !elevators.Any(x => x.GetOperationalState() == ElevatorOperationStatus.MOVING_UP))
                    {
                        availableElevator = elevator;
                        currentSmallestDifference = requestedFloor - elevator.GetCurrentFloor();
                    }
                }
                
            }

            return availableElevator;
        }

        private static bool ElevatorIsOnACloserFloor(int lowerFloor, int higherFloor, int currentSmallestDifference)
        {
            if (higherFloor - lowerFloor < currentSmallestDifference)
            {
                currentSmallestDifference = higherFloor - lowerFloor;
                return true;
            }

            return false;
        }

        private static int GetDifferenceBetweenFloors(int currentElevatorFloor, int requestedFloor)
        {
            return currentElevatorFloor > requestedFloor
                            ? currentElevatorFloor - requestedFloor
                            : requestedFloor - currentElevatorFloor;
        }
    }
}
