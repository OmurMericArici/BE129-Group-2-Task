using App.Api.Data.Services.Abstract;
using App.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentApiService _service;

        public CommentController(ICommentApiService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll().Value);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(int id)
        {
            var result = _service.GetById(id);
            if (!result.IsSuccess) return NotFound();
            return Ok(result.Value);
        }

        [HttpPost("approve/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Approve(int id)
        {
            var result = _service.Approve(id);
            if (!result.IsSuccess) return NotFound();
            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = "Buyer,Seller")]
        public IActionResult Create(CommentCreateDto model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _service.Create(userId, model);
            return Ok();
        }
    }
}