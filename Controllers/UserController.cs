using Microsoft.AspNetCore.Mvc;
using GestBurOrdAPI.Data;
using GestBurOrdAPI.Models;
using Microsoft.AspNetCore.Authorization;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using GestBurOrdAPI.DTOs;

namespace CrudUser.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDBcontext _context;
        private readonly string _jwtSecret;

        public UserController(AppDBcontext context, IConfiguration configuration)
        {
            _context = context;
            _jwtSecret = configuration["Jwt:SecretKey"];
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            if (dto.Password.Length < 6)
            {
                return BadRequest("Password must be at least 6 characters long.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                City = dto.City,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = passwordHash,
                Role = dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid email or password");
            }

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                Token = token,
                Role = user.Role
            });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized("User is not authenticated");
            }

            if (!int.TryParse(userIdClaim.Value, out int currentUserId))
            {
                return BadRequest("Invalid User ID in token");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == currentUserId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role
            });
        }



        
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto dto)
        {
          

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.City = dto.City;
            user.PhoneNumber = dto.PhoneNumber;
            user.Role = dto.Role;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("User updated successfully");
        }
        [HttpGet("userByid/{id}")]
        public async Task<IActionResult> getUserByID(int id)
        {


            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }


            return Ok(user);
        }



        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (currentUserId != id && !User.IsInRole("admin"))
            {
                return Forbid("You are not authorized to delete this user.");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("User deleted successfully");
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
