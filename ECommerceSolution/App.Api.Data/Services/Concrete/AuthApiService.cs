using App.Api.Data.Services.Abstract;
using App.Models.DTO;
using Ardalis.Result;
using Microsoft.IdentityModel.Tokens;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace App.Api.Data.Services.Concrete
{
    public class AuthApiService : IAuthApiService
    {
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IConfiguration _configuration;

        public AuthApiService(IRepository<UserEntity> userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public Result<LoginResponseDto> Login(LoginRequestDto model)
        {
            var user = _userRepository.Get(u => u.Email == model.Email && u.Password == model.Password);

            if (user == null) return Result.NotFound();
            if (!user.Enabled) return Result.Forbidden();

            var token = GenerateJwtToken(user);
            return Result.Success(new LoginResponseDto { Token = token });
        }

        public Result Register(RegisterRequestDto model)
        {
            if (_userRepository.Any(u => u.Email == model.Email))
            {
                return Result.Error("Email already exists.");
            }

            var entity = new UserEntity
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                RoleId = 1,
                Enabled = true,
                CreatedAt = DateTime.Now
            };

            _userRepository.Add(entity);
            return Result.Success();
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