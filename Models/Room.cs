using System.ComponentModel.DataAnnotations;

namespace RoomBookingTestTask.Models
{
    public class Room
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Description { get; set; } = string.Empty;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
