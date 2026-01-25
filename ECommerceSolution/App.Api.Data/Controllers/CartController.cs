using App.Api.Data.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartApiService _service;

        public CartController(ICartApiService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetCart()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Ok(_service.GetCart(userId).Value);
        }

        [HttpPost("add")]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = _service.AddToCart(userId, productId, quantity);
            if (!result.IsSuccess) return BadRequest(result.Errors);
            return Ok();
        }

        [HttpPut("update")]
        public IActionResult UpdateCart([FromBody] Dictionary<int, int> quantities)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _service.UpdateCart(userId, quantities);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveFromCart(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = _service.RemoveFromCart(userId, id);
            if (!result.IsSuccess) return NotFound();
            return Ok();
        }
    }
}