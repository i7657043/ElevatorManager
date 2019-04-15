using ElevatorProject.Enums;

namespace ElevatorProject.Models
{
    public class Request
    {
        public int Floor { get; set; }
        public int ElevatorId { get; set; }
        public RequestType RequestType { get; set; }
    }
}
