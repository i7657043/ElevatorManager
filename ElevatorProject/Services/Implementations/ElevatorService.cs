using ElevatorProject.Enums;
using ElevatorProject.Models;
using ElevatorProject.Models.Interfaces;
using ElevatorProject.Services.Interfaces;
using System.Collections.Generic;

namespace ElevatorProject.Services.Implementations
{
    public class ElevatorService : IElevatorService
    {
        //Get results from some persistent data store most likely, in reality
        public List<IElevator> GetElevators()
        {
            return new List<IElevator>
            {
                new Elevator(1, -3, 20, 13, 75),
                new Elevator(2, -3, 20, 6, 75)
            };
        }

        public void RaiseElevatorIssueAlert(int elevatorId)
        {
            //Raise an alert to a maintenance worker
            return;
        }
    }
}
