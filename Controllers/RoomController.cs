using GuestHouseAPI.Data;
using GuestHouseAPI.DTOs;
using GuestHouseAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuestHouseAPI.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoomController(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL ROOMS
        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _context.Rooms
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();

            return Ok(rooms);
        }

        // GET ROOM BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound(new
                {
                    Message = "Room not found"
                });
            }

            return Ok(room);
        }

        // GET AVAILABLE ROOMS
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableRooms()
        {
            var rooms = await _context.Rooms
                .Where(r => r.Status == "Available")
                .ToListAsync();

            return Ok(rooms);
        }

        // ADD ROOM
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AddRoom(RoomDTO dto)
        {
            var existingRoom = await _context.Rooms
                .FirstOrDefaultAsync(r => r.RoomNumber == dto.RoomNumber);

            if (existingRoom != null)
            {
                return BadRequest(new
                {
                    Message = "Room number already exists."
                });
            }

            var room = new Room
            {
                RoomNumber = dto.RoomNumber,
                RoomType = dto.RoomType,
                Status = dto.Status,
                Capacity = dto.Capacity
            };

            _context.Rooms.Add(room);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Room created successfully.",
                Room = room
            });
        }

        // UPDATE ROOM
        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, RoomDTO dto)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound(new
                {
                    Message = "Room not found."
                });
            }

            room.RoomNumber = dto.RoomNumber;
            room.RoomType = dto.RoomType;
            room.Status = dto.Status;
            room.Capacity = dto.Capacity;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Room updated successfully.",
                Room = room
            });
        }

        // MARK ROOM UNDER MAINTENANCE
        [AllowAnonymous]
        [HttpPut("maintenance/{id}")]
        public async Task<IActionResult> MarkRoomUnderMaintenance(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound(new
                {
                    Message = "Room not found."
                });
            }

            room.Status = "Maintenance";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Room marked as Maintenance."
            });
        }

        // MARK ROOM AVAILABLE
        [AllowAnonymous]
        [HttpPut("available/{id}")]
        public async Task<IActionResult> MarkRoomAvailable(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound(new
                {
                    Message = "Room not found."
                });
            }

            room.Status = "Available";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Room marked as Available."
            });
        }

        // DELETE ROOM
        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound(new
                {
                    Message = "Room not found."
                });
            }

            _context.Rooms.Remove(room);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Room deleted successfully."
            });
        }
    }
}