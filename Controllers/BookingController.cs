using GuestHouseAPI.Data;
using GuestHouseAPI.DTOs;
using GuestHouseAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuestHouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        // CREATE BOOKING
        [HttpPost]
        public async Task<IActionResult> CreateBooking(BookingDTO dto)
        {
            if (dto.CheckInDate >= dto.CheckOutDate)
            {
                return BadRequest("Check-out date must be after check-in date.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == dto.UserId);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var rooms = await _context.Rooms
                .Where(r =>
                    r.RoomType == dto.RoomType &&
                    r.Status == "Available")
                .ToListAsync();

            Room? availableRoom = null;

            foreach (var room in rooms)
            {
                bool hasConflict = await _context.Bookings.AnyAsync(b =>
                    b.RoomId == room.RoomId &&
                    b.Status != "Cancelled" &&
                    dto.CheckInDate < b.CheckOutDate &&
                    dto.CheckOutDate > b.CheckInDate);

                if (!hasConflict)
                {
                    availableRoom = room;
                    break;
                }
            }

            if (availableRoom == null)
            {
                return BadRequest("No available rooms for selected dates.");
            }

            var booking = new Booking
            {
                UserId = dto.UserId,
                RoomId = availableRoom.RoomId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                Status = "Pending"
            };

            _context.Bookings.Add(booking);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Booking created successfully.",
                BookingId = booking.BookingId,
                RoomAssigned = availableRoom.RoomNumber
            });
        }

        // GET ALL BOOKINGS
        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .ToListAsync();

            return Ok(bookings);
        }

        // GET BOOKING BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            return Ok(booking);
        }

        // APPROVE BOOKING
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            booking.Status = "Approved";

            await _context.SaveChangesAsync();

            return Ok("Booking approved successfully.");
        }

        // CHECK IN
        [HttpPut("checkin/{id}")]
        public async Task<IActionResult> CheckIn(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            booking.Status = "CheckedIn";

            if (booking.Room != null)
            {
                booking.Room.Status = "Occupied";
            }

            await _context.SaveChangesAsync();

            return Ok("Guest checked in successfully.");
        }

        // CHECK OUT
        [HttpPut("checkout/{id}")]
        public async Task<IActionResult> CheckOut(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            booking.Status = "CheckedOut";

            if (booking.Room != null)
            {
                booking.Room.Status = "Available";
            }

            await _context.SaveChangesAsync();

            return Ok("Guest checked out successfully.");
        }

        // CANCEL BOOKING
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            booking.Status = "Cancelled";

            await _context.SaveChangesAsync();

            return Ok("Booking cancelled successfully.");
        }

        // DELETE BOOKING
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            _context.Bookings.Remove(booking);

            await _context.SaveChangesAsync();

            return Ok("Booking deleted successfully.");
        }
    }
}