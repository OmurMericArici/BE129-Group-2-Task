using App.Api.Data.Services.Abstract;
using App.Models.DTO;
using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;

namespace App.Api.Data.Services.Concrete
{
    public class OrderApiService : IOrderApiService
    {
        private readonly IRepository<OrderEntity> _orderRepository;
        private readonly IRepository<CartItemEntity> _cartRepository;
        private readonly IRepository<OrderItemEntity> _orderItemRepository;
        private readonly IRepository<ProductEntity> _productRepository;
        private readonly ShoppersDbContext _context;

        public OrderApiService(
            IRepository<OrderEntity> orderRepository,
            IRepository<CartItemEntity> cartRepository,
            IRepository<OrderItemEntity> orderItemRepository,
            IRepository<ProductEntity> productRepository,
            ShoppersDbContext context)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
            _context = context;
        }

        public Result<List<OrderDto>> GetMyOrders(int userId)
        {
            var orders = _orderRepository.GetAll()
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderCode = o.OrderCode,
                    Address = o.Address,
                    CreatedAt = o.CreatedAt,
                    TotalPrice = o.OrderItems.Sum(i => i.Quantity * i.UnitPrice),
                    Items = o.OrderItems.Select(i => new OrderItemDto
                    {
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList()
                })
                .ToList();

            return Result.Success(orders);
        }

        public async Task<Result<OrderResponseDto>> CheckoutAsync(int userId, string address)
        {
            var cartItems = _cartRepository.GetAll()
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();

            if (!cartItems.Any()) return Result.Error("Cart is empty.");

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
                return Result.Success(new OrderResponseDto { OrderId = order.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result.Error(ex.Message);
            }
        }
    }
}