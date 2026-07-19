using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Api.Data;
using RoomBooking.Api.DTOs;
using RoomBooking.Api.Models;
using RoomBookingTestTask.DTOs;

namespace RoomBooking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(AppDbContext context, ILogger<BookingsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking([FromBody] CreateBookingDto dto)
        {
            var roomExists = await _context.Rooms.AnyAsync(r => r.Id == dto.RoomId);
            if (!roomExists) return NotFound("Такой комнаты не существует.");

            if (dto.StartTime <= DateTime.UtcNow || dto.EndTime <= DateTime.UtcNow)
            {
                return BadRequest("Время начала и окончания бронирования должны быть в будущем.");
            }

            if (dto.EndTime <= dto.StartTime)
            {
                return BadRequest("Время окончания должно быть позже времени начала.");
            }

            var duration = dto.EndTime - dto.StartTime;
            if (duration < TimeSpan.FromMinutes(15) || duration > TimeSpan.FromHours(8))
            {
                return BadRequest("Длительность бронирования должна быть от 15 минут до 8 часов.");
            }

            var hasOverlap = await _context.Bookings.AnyAsync(b =>
                b.RoomId == dto.RoomId &&
                b.StartTime < dto.EndTime &&
                dto.StartTime < b.EndTime);

            if (hasOverlap)
            {
                _logger.LogWarning("Попытка бронирования: Комната {RoomId} уже занята на интервал {Start} - {End}", dto.RoomId, dto.StartTime, dto.EndTime);
                return StatusCode(StatusCodes.Status409Conflict, "Комната уже занята на выбранное время.");
            }

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                RoomId = dto.RoomId,
                RenterName = dto.RenterName,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var response = new BookingResponseDto
            {
                Id = booking.Id,
                RoomId = booking.RoomId,
                RenterName = booking.RenterName,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime
            };

            _logger.LogInformation("Успешно создано бронирование {BookingId} для сотрудника {Renter} в комнату {RoomId}", booking.Id, booking.RenterName, booking.RoomId);
            return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, response);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingResponseDto>> GetBookingById(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound("Бронирование не найдено.");

            var response = new BookingResponseDto
            {
                Id = booking.Id,
                RoomId = booking.RoomId,
                RenterName = booking.RenterName,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound("Бронирование не найдено.");

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Бронирование {BookingId} было успешно отменено", id);
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetBookings(
            [FromQuery] Guid? roomId,
            [FromQuery] DateTime? date)
        {
            var quer = _context.Bookings.AsQueryable();

            if (roomId.HasValue)
            {
                quer = quer.Where(b => b.RoomId == roomId.Value);
            }

            if (date.HasValue)
            {
                var targetDate = date.Value.Date;
                quer = quer.Where(b => b.StartTime.Date == targetDate);
            }

            var bookings = await quer
                .Select(b => new BookingResponseDto
                {
                    Id = b.Id,
                    RoomId = b.RoomId,
                    RenterName = b.RenterName,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime
                })
                .ToListAsync();

            return Ok(bookings);

        }
    }
}
