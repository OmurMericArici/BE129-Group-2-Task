using App.Models.DTO;
using App.Services.Abstract;
using Ardalis.Result;

namespace App.Services.Concrete
{
    public class ProductService : BaseService, IProductService
    {
        public ProductService(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public async Task<Result<List<ProductDto>>> GetAllAsync()
        {
            return await SendRequestAsync<List<ProductDto>>("product", HttpMethod.Get);
        }

        public async Task<Result<ProductDto>> GetByIdAsync(int id)
        {
            return await SendRequestAsync<ProductDto>($"product/{id}", HttpMethod.Get);
        }

        public async Task<Result<List<ProductDto>>> GetMyProductsAsync(string jwt, int sellerId)
        {
            return await SendRequestAsync<List<ProductDto>>($"product/seller/{sellerId}", HttpMethod.Get, jwt);
        }

        public async Task<Result> CreateAsync(string jwt, ProductCreateDto model)
        {
            return await SendRequestAsync("product", HttpMethod.Post, jwt, model);
        }

        public async Task<Result> UpdateAsync(string jwt, ProductUpdateDto model)
        {
            return await SendRequestAsync("product", HttpMethod.Put, jwt, model);
        }

        public async Task<Result> DeleteAsync(string jwt, int id)
        {
            return await SendRequestAsync($"product/{id}", HttpMethod.Delete, jwt);
        }
    }
}