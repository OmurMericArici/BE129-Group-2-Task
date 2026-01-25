using App.Api.Data.Services.Abstract;
using App.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserApiService _service;

        public UserController(IUserApiService service)
        {
            _service = service;
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetMe()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = _service.GetUser(userId);
            if (!result.IsSuccess) return NotFound();
            return Ok(result.Value);
        }

        [HttpPut("me")]
        [Authorize]
        public IActionResult UpdateMe(UserUpdateDto model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = _service.UpdateUser(userId, model);
            if (!result.IsSuccess) return NotFound();
            return Ok(result.Value);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAllUsers().Value);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(int id)
        {
            var result = _service.GetUser(id);
            if (!result.IsSuccess) return NotFound();
            return Ok(result.Value);
        }

        [HttpPost("approve/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult ApproveSeller(int id)
        {
            var result = _service.ApproveSeller(id);
            if (!result.IsSuccess) return NotFound();
            return Ok();
        }
    }
}