using App.Api.Data.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Buyer,Seller")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderApiService _service;

        public OrderController(IOrderApiService service)
        {
            _service = service;
        }

        [HttpGet("myorders")]
        public IActionResult GetMyOrders()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Ok(_service.GetMyOrders(userId).Value);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] string address)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _service.CheckoutAsync(userId, address);

            if (!result.IsSuccess) return BadRequest(result.Errors);
            return Ok(result.Value);
        }
    }
}