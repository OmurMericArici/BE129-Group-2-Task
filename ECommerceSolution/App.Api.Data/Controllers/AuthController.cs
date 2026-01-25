using App.Api.Data.Services.Abstract;
using App.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthApiService _service;

        public AuthController(IAuthApiService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequestDto model)
        {
            var result = _service.Login(model);
            if (!result.IsSuccess)
            {
                return Unauthorized(result.Errors);
            }
            return Ok(result.Value);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] RegisterRequestDto model)
        {
            var result = _service.Register(model);
            if (!result.IsSuccess) return BadRequest(result.Errors);
            return Ok(new { Message = "Registration successful" });
        }
    }
}