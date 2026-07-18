namespace RoomBooking.Api.DTOs
{
    public class BookingResponseDto
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public string RenterName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
