using App.Api.Data.Services.Abstract;
using App.Models.DTO;
using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;

namespace App.Api.Data.Services.Concrete
{
    public class CartApiService : ICartApiService
    {
        private readonly IRepository<CartItemEntity> _cartRepository;
        private readonly IRepository<ProductEntity> _productRepository;

        public CartApiService(IRepository<CartItemEntity> cartRepository, IRepository<ProductEntity> productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public Result<List<CartItemDto>> GetCart(int userId)
        {
            var items = _cartRepository.GetAll()
                .Include(c => c.Product)
                .ThenInclude(p => p.Images)
                .Where(c => c.UserId == userId)
                .Select(c => new CartItemDto
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    Price = c.Product.Price,
                    Quantity = c.Quantity,
                    StockAvailable = c.Product.StockAmount,
                    ImageUrl = c.Product.Images.Select(i => i.Url).FirstOrDefault()
                })
                .ToList();

            return Result.Success(items);
        }

        public Result AddToCart(int userId, int productId, int quantity)
        {
            var product = _productRepository.GetById(productId);
            if (product == null) return Result.NotFound();
            if (product.StockAmount < quantity) return Result.Error("Insufficient stock.");

            var cartItem = _cartRepository.Get(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem != null)
            {
                if (product.StockAmount >= cartItem.Quantity + quantity)
                {
                    cartItem.Quantity += (byte)quantity;
                    _cartRepository.Update(cartItem);
                }
                else return Result.Error("Stock limit reached.");
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
            return Result.Success();
        }

        public Result UpdateCart(int userId, Dictionary<int, int> quantities)
        {
            foreach (var q in quantities)
            {
                var item = _cartRepository.GetAll().Include(c => c.Product).FirstOrDefault(c => c.Id == q.Key && c.UserId == userId);
                if (item != null && q.Value > 0 && q.Value <= item.Product.StockAmount)
                {
                    item.Quantity = (byte)q.Value;
                    _cartRepository.Update(item);
                }
            }
            return Result.Success();
        }

        public Result RemoveFromCart(int userId, int cartItemId)
        {
            var item = _cartRepository.Get(c => c.Id == cartItemId && c.UserId == userId);
            if (item != null)
            {
                _cartRepository.Delete(item);
                return Result.Success();
            }
            return Result.NotFound();
        }
    }
}