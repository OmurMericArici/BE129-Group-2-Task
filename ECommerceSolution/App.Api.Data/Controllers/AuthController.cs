using App.Api.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IRepository<UserEntity> userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginDto model)
        {
            var user = _userRepository.Get(u => u.Email == model.Email && u.Password == model.Password);

            if (user == null)
            {
                return NotFound("User not found or invalid credentials.");
            }

            if (!user.Enabled)
            {
                return Unauthorized("Account is disabled.");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] UserEntity model)
        {
            if (_userRepository.Any(u => u.Email == model.Email))
            {
                return BadRequest("Email already exists.");
            }
            model.RoleId = 1;
            model.Enabled = true;
            model.CreatedAt = DateTime.Now;
            _userRepository.Add(model);

            return Ok(new { Message = "Registration successful" });
        }

        private string GenerateJwtToken(UserEntity user)
        {
            var roleName = user.RoleId == 3 ? "Admin" : (user.RoleId == 2 ? "Seller" : "Buyer");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("FullName", $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, roleName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}