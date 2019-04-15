using System;
using System.Collections.Generic;
using System.Text;

namespace ElevatorProject.Enums
{
    public enum ElevatorOperationStatus
    {
        MAINTENANCE_MODE = 0, AVAILABLE = 1, MOVING_UP = 2, MOVING_DOWN = 3, OVERWEIGHT = 4, RUNNING_ENROUTE_PICKUP_ROUTINE = 5
    }
}
