using ElevatorProject.Models;
using ElevatorProject.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElevatorProject.Services.Interfaces
{
    public interface IElevatorService
    {
        List<IElevator> GetElevators();
        void RaiseElevatorIssueAlert(int elevatorId);
    }
}
