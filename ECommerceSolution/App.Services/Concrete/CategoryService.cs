using App.Models.DTO;
using App.Services.Abstract;
using Ardalis.Result;

namespace App.Services.Concrete
{
    public class CategoryService : BaseService, ICategoryService
    {
        public CategoryService(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public async Task<Result<List<CategoryDto>>> GetAllAsync()
        {
            return await SendRequestAsync<List<CategoryDto>>("category", HttpMethod.Get);
        }

        public async Task<Result<CategoryDto>> GetByIdAsync(int id)
        {
            return await SendRequestAsync<CategoryDto>($"category/{id}", HttpMethod.Get);
        }

        public async Task<Result> CreateAsync(string jwt, CategoryCreateDto model)
        {
            return await SendRequestAsync("category", HttpMethod.Post, jwt, model);
        }

        public async Task<Result> UpdateAsync(string jwt, CategoryUpdateDto model)
        {
            return await SendRequestAsync("category", HttpMethod.Put, jwt, model);
        }

        public async Task<Result> DeleteAsync(string jwt, int id)
        {
            return await SendRequestAsync($"category/{id}", HttpMethod.Delete, jwt);
        }
    }
}