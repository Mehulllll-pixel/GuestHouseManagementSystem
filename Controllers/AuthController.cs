using GuestHouseAPI.Data;
using GuestHouseAPI.DTOs;
using GuestHouseAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GuestHouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(
            AppDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // REGISTER
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (existingUser != null)
            {
                return BadRequest("Email already exists.");
            }

            // Guest RoleId = 5
            if (dto.RoleId != 5 &&
                string.IsNullOrWhiteSpace(dto.EmployeeId))
            {
                return BadRequest("Employee ID is required.");
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                PhoneNumber = dto.PhoneNumber,
                EmployeeId = dto.EmployeeId,
                RoleId = dto.RoleId
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "User registered successfully."
            });
        }

        // LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                return Unauthorized("Invalid Email");
            }

            var hashedPassword = HashPassword(dto.Password);

            if (user.PasswordHash != hashedPassword)
            {
                return Unauthorized("Invalid Password");
            }

            // Employee roles require Employee ID
            if (user.RoleId != 5)
            {
                if (string.IsNullOrWhiteSpace(dto.EmployeeId))
                {
                    return Unauthorized("Employee ID is required");
                }

                if (user.EmployeeId != dto.EmployeeId)
                {
                    return Unauthorized("Invalid Employee ID");
                }
            }

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                Token = token,
                user.UserId,
                user.FullName,
                user.Email,
                user.PhoneNumber,
                user.EmployeeId,
                Role = user.Role?.RoleName,
                Message = "Login Successful"
            });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserId.ToString()),

                new Claim(
                    ClaimTypes.Email,
                    user.Email),

                new Claim(
                    ClaimTypes.Role,
                    user.Role?.RoleName ?? "")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using SHA256 sha = SHA256.Create();

            var bytes = sha.ComputeHash(
                Encoding.UTF8.GetBytes(password));

            return Convert.ToBase64String(bytes);
        }
    }
}