using App.Models.DTO;
using Ardalis.Result;

namespace App.Api.Data.Services.Abstract
{
    public interface ICategoryApiService
    {
        Result<List<CategoryDto>> GetAll();
        Result<CategoryDto> GetById(int id);
        Result Create(CategoryCreateDto model);
        Result Update(CategoryUpdateDto model);
        Result Delete(int id);
    }
}