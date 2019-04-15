using ElevatorProject.Enums;
using ElevatorProject.Implementations;
using ElevatorProject.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorProject.Interfaces
{
    public interface IElevatorManager
    {
        void MakeElevatorManagerRequest(Request request);
        Task<int> RunElevatorManager();
    }
}
