using App.Models.DTO;
using App.Services.Abstract;
using Ardalis.Result;

namespace App.Services.Concrete
{
    public class OrderService : BaseService, IOrderService
    {
        public OrderService(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public async Task<Result<List<OrderDto>>> GetMyOrdersAsync(string jwt)
        {
            return await SendRequestAsync<List<OrderDto>>("order/myorders", HttpMethod.Get, jwt);
        }

        public async Task<Result<OrderResponseDto>> CreateOrderAsync(string jwt, OrderCreateRequestDto model)
        {
            return await SendRequestAsync<OrderResponseDto>("order/checkout", HttpMethod.Post, jwt, model.Address);
        }
    }
}