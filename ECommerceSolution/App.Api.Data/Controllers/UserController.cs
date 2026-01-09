using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;
using System.Security.Claims;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRepository<UserEntity> _userRepository;

        public UserController(IRepository<UserEntity> userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetMe()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = _userRepository.GetAll().Include(u => u.Role).FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("me")]
        [Authorize]
        public IActionResult UpdateMe(UserEntity model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = _userRepository.GetById(userId);
            if (user == null) return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            _userRepository.Update(user);
            return Ok(user);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            var users = _userRepository.GetAll().Include(u => u.Role).OrderByDescending(u => u.CreatedAt).ToList();
            return Ok(users);
        }

        [HttpPost("approve/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult ApproveSeller(int id)
        {
            var user = _userRepository.GetById(id);
            if (user == null) return NotFound();

            user.RoleId = 2;
            _userRepository.Update(user);
            return Ok();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(int id)
        {
            var user = _userRepository.GetAll().Include(u => u.Role).FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();
            return Ok(user);
        }
    }
}