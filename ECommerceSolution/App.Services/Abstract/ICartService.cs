using App.Models.DTO;
using Ardalis.Result;

namespace App.Services.Abstract
{
    public interface ICartService
    {
        Task<Result<List<CartItemDto>>> GetCartAsync(string jwt);
        Task<Result> AddToCartAsync(string jwt, int productId, int quantity);
        Task<Result> UpdateCartAsync(string jwt, Dictionary<int, int> quantities);
        Task<Result> RemoveFromCartAsync(string jwt, int cartItemId);
    }
}