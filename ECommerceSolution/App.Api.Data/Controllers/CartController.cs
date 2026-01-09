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
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly IRepository<CartItemEntity> _cartRepository;
        private readonly IRepository<ProductEntity> _productRepository;

        public CartController(IRepository<CartItemEntity> cartRepository, IRepository<ProductEntity> productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        [HttpGet]
        public IActionResult GetCart()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var items = _cartRepository.GetAll()
                .Include(c => c.Product)
                .ThenInclude(p => p.Images)
                .Where(c => c.UserId == userId)
                .ToList();
            return Ok(items);
        }

        [HttpPost("add")]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var product = _productRepository.GetById(productId);

            if (product == null) return NotFound("Product not found.");
            if (product.StockAmount < quantity) return BadRequest("Insufficient stock.");

            var cartItem = _cartRepository.Get(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem != null)
            {
                if (product.StockAmount >= cartItem.Quantity + quantity)
                {
                    cartItem.Quantity += (byte)quantity;
                    _cartRepository.Update(cartItem);
                }
                else return BadRequest("Stock limit reached.");
            }
            else
            {
                _cartRepository.Add(new CartItemEntity
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = (byte)quantity,
                    CreatedAt = DateTime.Now
                });
            }
            return Ok();
        }

        [HttpPut("update")]
        public IActionResult UpdateCart([FromBody] Dictionary<int, int> quantities)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            foreach (var q in quantities)
            {
                var item = _cartRepository.GetAll().Include(c => c.Product).FirstOrDefault(c => c.Id == q.Key && c.UserId == userId);
                if (item != null && q.Value > 0 && q.Value <= item.Product.StockAmount)
                {
                    item.Quantity = (byte)q.Value;
                    _cartRepository.Update(item);
                }
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveFromCart(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var item = _cartRepository.Get(c => c.Id == id && c.UserId == userId);
            if (item != null)
            {
                _cartRepository.Delete(item);
                return Ok();
            }
            return NotFound();
        }
    }
}