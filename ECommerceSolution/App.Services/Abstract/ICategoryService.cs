using App.Models.DTO;
using Ardalis.Result;

namespace App.Services.Abstract
{
    public interface ICategoryService
    {
        Task<Result<List<CategoryDto>>> GetAllAsync();
        Task<Result<CategoryDto>> GetByIdAsync(int id);
        Task<Result> CreateAsync(string jwt, CategoryCreateDto model);
        Task<Result> UpdateAsync(string jwt, CategoryUpdateDto model);
        Task<Result> DeleteAsync(string jwt, int id);
    }
}