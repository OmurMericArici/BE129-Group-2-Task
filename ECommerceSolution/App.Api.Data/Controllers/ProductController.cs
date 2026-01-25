using App.Api.Data.Services.Abstract;
using App.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductApiService _service;

        public ProductController(IProductApiService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll().Value);
        }

        [HttpGet("seller/{sellerId}")]
        [Authorize(Roles = "Seller")]
        public IActionResult GetBySeller(int sellerId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (currentUserId != sellerId) return Forbid();

            return Ok(_service.GetBySeller(sellerId).Value);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = _service.GetById(id);
            if (!result.IsSuccess) return NotFound();
            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public IActionResult Create(ProductCreateDto model)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _service.Create(currentUserId, model);
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "Seller")]
        public IActionResult Update(ProductUpdateDto model)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = _service.Update(currentUserId, model);
            if (result.Status == Ardalis.Result.ResultStatus.Forbidden) return Forbid();
            if (result.Status == Ardalis.Result.ResultStatus.NotFound) return NotFound();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller,Admin")]
        public IActionResult Delete(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            var result = _service.Delete(currentUserId, userRole, id);

            if (result.Status == Ardalis.Result.ResultStatus.Forbidden) return Forbid();
            if (result.Status == Ardalis.Result.ResultStatus.NotFound) return NotFound();

            return NoContent();
        }
    }
}