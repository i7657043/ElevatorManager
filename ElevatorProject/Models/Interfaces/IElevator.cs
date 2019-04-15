using ElevatorProject.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorProject.Models.Interfaces
{
    public interface IElevator
    {
        bool CloseDoors();
        int GetBottomFloor();
        int GetCurrentFloor();
        int GetId();
        ElevatorOperationStatus GetOperationalState();            
        int GetTopFloor();
        Task MoveBetweenFloors();
        bool OpenDoors();
        void ReadyElevatorForJourney(Request request);
        void ReadyElevatorForNewPassengers();
        void RunEnroutePickupRoutine(Request request);
        void SetElevatorAsAvailable();
        void SetToMaintenanceMode();
        bool RunSafetyChecks();
        bool WeighElevator();
    }
}
