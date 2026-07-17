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
    public class RoomsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoomsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<RoomResponseDto>> CreateRoom([FromBody] CreateRoomDto dto)
        {
            var nameExists = await _context.Rooms.AnyAsync(r => r.Name.ToLower() == dto.Name.ToLower());
            if (nameExists)
            {
                return BadRequest("Комната с таким именем уже есть.");

            }

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Capacity = dto.Capacity,
                Description = dto.Description
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var response = new RoomResponseDto
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                Description = room.Description
            };

            return CreatedAtAction(nameof(GetRoomById), new { id = room.Id }, response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomResponseDto>>> GetAllRooms()
        {
            var rooms = await _context.Rooms
                .Select(r => new RoomResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Capacity = r.Capacity,
                    Description = r.Description
                })
                .ToListAsync();
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomResponseDto>> GetRoomById(Guid id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound("Комната не найдена.");

            var response = new RoomResponseDto
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                Description = room.Description
            };

            return Ok(response);
        }
    }
}
