using App.Api.Data.Services.Abstract;
using App.Models.DTO;
using Ardalis.Result;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;

namespace App.Api.Data.Services.Concrete
{
    public class CategoryApiService : ICategoryApiService
    {
        private readonly IRepository<CategoryEntity> _repository;

        public CategoryApiService(IRepository<CategoryEntity> repository)
        {
            _repository = repository;
        }

        public Result<List<CategoryDto>> GetAll()
        {
            var entities = _repository.GetAll().OrderBy(c => c.Name).ToList();
            var dtos = entities.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color,
                IconCssClass = c.IconCssClass
            }).ToList();
            return Result.Success(dtos);
        }

        public Result<CategoryDto> GetById(int id)
        {
            var c = _repository.GetById(id);
            if (c == null) return Result.NotFound();

            return Result.Success(new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color,
                IconCssClass = c.IconCssClass
            });
        }

        public Result Create(CategoryCreateDto model)
        {
            var entity = new CategoryEntity
            {
                Name = model.Name,
                Color = model.Color,
                IconCssClass = model.IconCssClass,
                CreatedAt = DateTime.Now
            };
            _repository.Add(entity);
            return Result.Success();
        }

        public Result Update(CategoryUpdateDto model)
        {
            var entity = _repository.GetById(model.Id);
            if (entity == null) return Result.NotFound();

            entity.Name = model.Name;
            entity.Color = model.Color;
            entity.IconCssClass = model.IconCssClass;

            _repository.Update(entity);
            return Result.Success();
        }

        public Result Delete(int id)
        {
            var entity = _repository.GetById(id);
            if (entity == null) return Result.NotFound();

            _repository.Delete(entity);
            return Result.Success();
        }
    }
}