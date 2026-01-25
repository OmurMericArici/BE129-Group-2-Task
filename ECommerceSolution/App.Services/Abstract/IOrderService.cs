using App.Models.DTO;
using Ardalis.Result;

namespace App.Services.Abstract
{
    public interface IOrderService
    {
        Task<Result<List<OrderDto>>> GetMyOrdersAsync(string jwt);
        Task<Result<OrderResponseDto>> CreateOrderAsync(string jwt, OrderCreateRequestDto model);
    }
}