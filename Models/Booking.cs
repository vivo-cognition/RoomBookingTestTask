using System.ComponentModel.DataAnnotations;
using System.Data;

namespace RoomBooking.Api.Models
{
    public class Booking
    {
        public Guid Id { get; set; }
        [Required]
        public Guid RoomId { get; set; }
        public string RenterName { get; set; } = string.Empty;
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public Room? Room { get; set; }
    }
}
