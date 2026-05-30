using GuestHouseAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuestHouseAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Management")]
        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var totalRooms = await _context.Rooms.CountAsync();

            var availableRooms =
                await _context.Rooms.CountAsync(r =>
                    r.Status == "Available");

            var occupiedRooms =
                await _context.Rooms.CountAsync(r =>
                    r.Status == "Occupied");

            var maintenanceRooms =
                await _context.Rooms.CountAsync(r =>
                    r.Status == "Maintenance");

            var totalBookings =
                await _context.Bookings.CountAsync();

            return Ok(new
            {
                TotalRooms = totalRooms,
                AvailableRooms = availableRooms,
                OccupiedRooms = occupiedRooms,
                MaintenanceRooms = maintenanceRooms,
                TotalBookings = totalBookings
            });
        }

        [Authorize(Roles = "Admin,Management")]
        [HttpGet("monthly-bookings")]
        public async Task<IActionResult> GetMonthlyBookings()
        {
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;

            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .Where(b =>
                    b.CheckInDate.Month == month &&
                    b.CheckInDate.Year == year)
                .ToListAsync();

            return Ok(bookings);
        }

        [Authorize(Roles = "Admin,Management")]
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetBookingsByEmployee(
            string employeeId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .Where(b =>
                    b.User.EmployeeId == employeeId)
                .ToListAsync();

            return Ok(bookings);
        }

        [Authorize(Roles = "Admin,Management")]
        [HttpGet("room/{roomNumber}")]
        public async Task<IActionResult> GetBookingsByRoom(
            string roomNumber)
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .Where(b =>
                    b.Room.RoomNumber == roomNumber)
                .ToListAsync();

            return Ok(bookings);
        }

        [Authorize(Roles = "Admin,Management,Security,Staff")]
        [HttpGet("available-rooms")]
        public async Task<IActionResult> GetAvailableRooms()
        {
            return Ok(await _context.Rooms
                .Where(r => r.Status == "Available")
                .ToListAsync());
        }

        [Authorize(Roles = "Admin,Management,Security,Staff")]
        [HttpGet("occupied-rooms")]
        public async Task<IActionResult> GetOccupiedRooms()
        {
            return Ok(await _context.Rooms
                .Where(r => r.Status == "Occupied")
                .ToListAsync());
        }

        [Authorize(Roles = "Admin,Management,Security,Staff")]
        [HttpGet("maintenance-rooms")]
        public async Task<IActionResult> GetMaintenanceRooms()
        {
            return Ok(await _context.Rooms
                .Where(r => r.Status == "Maintenance")
                .ToListAsync());
        }
    }
}