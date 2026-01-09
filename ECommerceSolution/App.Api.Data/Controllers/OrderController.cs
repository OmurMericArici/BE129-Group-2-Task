using App.Api.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;
using System.Security.Claims;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Buyer,Seller")]
    public class OrderController : ControllerBase
    {
        private readonly IRepository<OrderEntity> _orderRepository;
        private readonly IRepository<OrderItemEntity> _orderItemRepository;
        private readonly IRepository<CartItemEntity> _cartRepository;
        private readonly IRepository<ProductEntity> _productRepository;
        private readonly ShoppersDbContext _context;

        public OrderController(
            IRepository<OrderEntity> orderRepository,
            IRepository<OrderItemEntity> orderItemRepository,
            IRepository<CartItemEntity> cartRepository,
            IRepository<ProductEntity> productRepository,
            ShoppersDbContext context)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _context = context;
        }

        [HttpGet("myorders")]
        public IActionResult GetMyOrders()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var orders = _orderRepository.GetAll()
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();
            return Ok(orders);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] string address)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var cartItems = _cartRepository.GetAll()
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();

            if (!cartItems.Any()) return BadRequest("Cart is empty.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new OrderEntity
                {
                    UserId = userId,
                    Address = address,
                    OrderCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    CreatedAt = DateTime.Now
                };
                _orderRepository.Add(order);

                foreach (var item in cartItems)
                {
                    if (item.Product.StockAmount < item.Quantity)
                        throw new Exception($"Insufficient stock for {item.Product.Name}");

                    _orderItemRepository.Add(new OrderItemEntity
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price,
                        CreatedAt = DateTime.Now
                    });

                    item.Product.StockAmount -= item.Quantity;
                    _productRepository.Update(item.Product);
                }

                foreach (var item in cartItems) _cartRepository.Delete(item);

                await transaction.CommitAsync();
                return Ok(new { OrderId = order.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }
    }
}