using GuestHouseAPI.Data;
using GuestHouseAPI.DTOs;
using GuestHouseAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuestHouseAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MaintenanceController(AppDbContext context)
        {
            _context = context;
        }

        // CREATE MAINTENANCE REQUEST
        [Authorize(Roles = "Staff,Management,Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateMaintenance(MaintenanceDTO dto)
        {
            var room = await _context.Rooms.FindAsync(dto.RoomId);

            if (room == null)
            {
                return NotFound(new
                {
                    Message = "Room not found."
                });
            }

            var user = await _context.Users
                .FindAsync(dto.ReportedByUserId);

            if (user == null)
            {
                return NotFound(new
                {
                    Message = "User not found."
                });
            }

            room.Status = "Maintenance";

            var maintenance = new Maintenance
            {
                RoomId = dto.RoomId,
                ReportedByUserId = dto.ReportedByUserId,
                Reason = dto.Reason,
                Status = "Open"
            };

            _context.Maintenances.Add(maintenance);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Maintenance request created successfully.",
                MaintenanceId = maintenance.MaintenanceId,
                RoomNumber = room.RoomNumber,
                Status = maintenance.Status
            });
        }

        // GET ALL MAINTENANCE REQUESTS
        [Authorize(Roles = "Admin,Management,Staff")]
        [HttpGet]
        public async Task<IActionResult> GetMaintenanceRequests()
        {
            var requests = await _context.Maintenances
                .Include(m => m.Room)
                .Include(m => m.ReportedByUser)
                .Select(m => new
                {
                    m.MaintenanceId,
                    RoomNumber = m.Room.RoomNumber,
                    ReportedBy = m.ReportedByUser.FullName,
                    m.Reason,
                    m.Status,
                    m.CreatedAt
                })
                .ToListAsync();

            return Ok(requests);
        }

        // GET MAINTENANCE BY ID
        [Authorize(Roles = "Admin,Management,Staff")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaintenanceById(int id)
        {
            var maintenance = await _context.Maintenances
                .Include(m => m.Room)
                .Include(m => m.ReportedByUser)
                .FirstOrDefaultAsync(m => m.MaintenanceId == id);

            if (maintenance == null)
            {
                return NotFound(new
                {
                    Message = "Maintenance record not found."
                });
            }

            return Ok(maintenance);
        }

        // COMPLETE MAINTENANCE
        [Authorize(Roles = "Management,Admin")]
        [HttpPut("complete/{id}")]
        public async Task<IActionResult> CompleteMaintenance(int id)
        {
            var maintenance = await _context.Maintenances
                .Include(m => m.Room)
                .FirstOrDefaultAsync(m => m.MaintenanceId == id);

            if (maintenance == null)
            {
                return NotFound(new
                {
                    Message = "Maintenance record not found."
                });
            }

            maintenance.Status = "Completed";

            if (maintenance.Room != null)
            {
                maintenance.Room.Status = "Available";
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Maintenance completed successfully.",
                RoomNumber = maintenance.Room?.RoomNumber
            });
        }

        // DELETE MAINTENANCE RECORD
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaintenance(int id)
        {
            var maintenance = await _context.Maintenances
                .FindAsync(id);

            if (maintenance == null)
            {
                return NotFound(new
                {
                    Message = "Maintenance record not found."
                });
            }

            _context.Maintenances.Remove(maintenance);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Maintenance record deleted successfully."
            });
        }
    }
}