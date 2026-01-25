using App.Models.DTO;
using Ardalis.Result;

namespace App.Api.Data.Services.Abstract
{
    public interface IProductApiService
    {
        Result<List<ProductDto>> GetAll();
        Result<ProductDto> GetById(int id);
        Result<List<ProductDto>> GetBySeller(int sellerId);
        Result Create(int sellerId, ProductCreateDto model);
        Result Update(int sellerId, ProductUpdateDto model);
        Result Delete(int userId, string userRole, int productId);
    }
}