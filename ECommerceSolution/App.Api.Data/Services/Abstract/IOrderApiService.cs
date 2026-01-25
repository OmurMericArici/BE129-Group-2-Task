using App.Models.DTO;
using Ardalis.Result;

namespace App.Api.Data.Services.Abstract
{
    public interface IOrderApiService
    {
        Result<List<OrderDto>> GetMyOrders(int userId);
        Task<Result<OrderResponseDto>> CheckoutAsync(int userId, string address);
    }
}