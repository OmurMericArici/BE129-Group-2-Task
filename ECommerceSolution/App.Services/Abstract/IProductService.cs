using App.Models.DTO;
using Ardalis.Result;

namespace App.Services.Abstract
{
    public interface IProductService
    {
        Task<Result<List<ProductDto>>> GetAllAsync();
        Task<Result<ProductDto>> GetByIdAsync(int id);
        Task<Result<List<ProductDto>>> GetMyProductsAsync(string jwt, int sellerId);
        Task<Result> CreateAsync(string jwt, ProductCreateDto model);
        Task<Result> UpdateAsync(string jwt, ProductUpdateDto model);
        Task<Result> DeleteAsync(string jwt, int id);
    }
}