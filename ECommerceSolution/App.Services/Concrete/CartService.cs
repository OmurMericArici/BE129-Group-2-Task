using App.Models.DTO;
using App.Services.Abstract;
using Ardalis.Result;

namespace App.Services.Concrete
{
    public class CartService : BaseService, ICartService
    {
        public CartService(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public async Task<Result<List<CartItemDto>>> GetCartAsync(string jwt)
        {
            return await SendRequestAsync<List<CartItemDto>>("cart", HttpMethod.Get, jwt);
        }

        public async Task<Result> AddToCartAsync(string jwt, int productId, int quantity)
        {
            return await SendRequestAsync($"cart/add?productId={productId}&quantity={quantity}", HttpMethod.Post, jwt);
        }

        public async Task<Result> UpdateCartAsync(string jwt, Dictionary<int, int> quantities)
        {
            return await SendRequestAsync("cart/update", HttpMethod.Put, jwt, quantities);
        }

        public async Task<Result> RemoveFromCartAsync(string jwt, int cartItemId)
        {
            return await SendRequestAsync($"cart/{cartItemId}", HttpMethod.Delete, jwt);
        }
    }
}