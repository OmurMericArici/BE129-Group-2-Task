using App.Models.DTO;
using Ardalis.Result;

namespace App.Api.Data.Services.Abstract
{
    public interface ICartApiService
    {
        Result<List<CartItemDto>> GetCart(int userId);
        Result AddToCart(int userId, int productId, int quantity);
        Result UpdateCart(int userId, Dictionary<int, int> quantities);
        Result RemoveFromCart(int userId, int cartItemId);
    }
}