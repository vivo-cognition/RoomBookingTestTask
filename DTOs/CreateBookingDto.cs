using System.ComponentModel.DataAnnotations;

namespace RoomBooking.Api.DTOs
{
    public class CreateBookingDto
    {
        [Required]
        public Guid RoomId { get; set; }

        [Required(ErrorMessage = "Имя сотрудника обязательно")]
        public string RenterName { get; set; } = string.Empty;
        
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
