using App.Api.Data.Services.Abstract;
using App.Models.DTO;
using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;

namespace App.Api.Data.Services.Concrete
{
    public class ProductApiService : IProductApiService
    {
        private readonly IRepository<ProductEntity> _repository;

        public ProductApiService(IRepository<ProductEntity> repository)
        {
            _repository = repository;
        }

        public Result<List<ProductDto>> GetAll()
        {
            var products = _repository.GetAll()
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.Enabled)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            var dtos = products.Select(p => MapToDto(p)).ToList();
            return Result.Success(dtos);
        }

        public Result<ProductDto> GetById(int id)
        {
            var p = _repository.GetAll()
                .Include(c => c.Category)
                .Include(i => i.Images)
                .FirstOrDefault(x => x.Id == id);

            if (p == null) return Result.NotFound();
            return Result.Success(MapToDto(p));
        }

        public Result<List<ProductDto>> GetBySeller(int sellerId)
        {
            var products = _repository.GetAll()
                .Include(p => p.Images)
                .Where(p => p.SellerId == sellerId && p.Enabled)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            var dtos = products.Select(p => MapToDto(p)).ToList();
            return Result.Success(dtos);
        }

        public Result Create(int sellerId, ProductCreateDto model)
        {
            var entity = new ProductEntity
            {
                SellerId = sellerId,
                CategoryId = model.CategoryId,
                Name = model.Name,
                Price = model.Price,
                StockAmount = (byte)model.StockAmount,
                Details = model.Details,
                CreatedAt = DateTime.Now,
                Enabled = true,
                Images = new List<ProductImageEntity>()
            };

            if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                entity.Images.Add(new ProductImageEntity
                {
                    Url = model.ImageUrl,
                    CreatedAt = DateTime.Now
                });
            }

            _repository.Add(entity);
            return Result.Success();
        }

        public Result Update(int sellerId, ProductUpdateDto model)
        {
            var existing = _repository.GetAll().Include(p => p.Images).FirstOrDefault(p => p.Id == model.Id);
            if (existing == null) return Result.NotFound();
            if (existing.SellerId != sellerId) return Result.Forbidden();

            existing.Name = model.Name;
            existing.Price = model.Price;
            existing.StockAmount = (byte)model.StockAmount;
            existing.Details = model.Details;
            existing.CategoryId = model.CategoryId;

            if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                if (existing.Images == null) existing.Images = new List<ProductImageEntity>();

                var img = existing.Images.FirstOrDefault();
                if (img != null) img.Url = model.ImageUrl;
                else existing.Images.Add(new ProductImageEntity { Url = model.ImageUrl, CreatedAt = DateTime.Now });
            }

            _repository.Update(existing);
            return Result.Success();
        }

        public Result Delete(int userId, string userRole, int productId)
        {
            var product = _repository.GetById(productId);
            if (product == null) return Result.NotFound();

            if (userRole == "Seller" && product.SellerId != userId)
            {
                return Result.Forbidden();
            }

            product.Enabled = false;
            _repository.Update(product);
            return Result.Success();
        }

        private static ProductDto MapToDto(ProductEntity p)
        {
            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                StockAmount = p.StockAmount,
                Details = p.Details,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                SellerId = p.SellerId,
                Enabled = p.Enabled,
                ImageUrls = p.Images?.Select(i => i.Url).ToList() ?? new List<string>()
            };
        }
    }
}