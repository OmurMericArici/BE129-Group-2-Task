using App.Api.Data.Services.Abstract;
using App.Models.DTO;
using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;

namespace App.Api.Data.Services.Concrete
{
    public class CommentApiService : ICommentApiService
    {
        private readonly IRepository<ProductCommentEntity> _repository;

        public CommentApiService(IRepository<ProductCommentEntity> repository)
        {
            _repository = repository;
        }

        public Result<List<CommentDto>> GetAll()
        {
            var comments = _repository.GetAll()
                .Include(c => c.Product)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    UserId = c.UserId,
                    UserName = $"{c.User.FirstName} {c.User.LastName}",
                    Text = c.Text,
                    StarCount = c.StarCount,
                    IsConfirmed = c.IsConfirmed,
                    CreatedAt = c.CreatedAt
                })
                .ToList();
            return Result.Success(comments);
        }

        public Result<CommentDto> GetById(int id)
        {
            var c = _repository.GetAll()
                .Include(x => x.Product)
                .Include(x => x.User)
                .FirstOrDefault(x => x.Id == id);

            if (c == null) return Result.NotFound();

            return Result.Success(new CommentDto
            {
                Id = c.Id,
                ProductId = c.ProductId,
                ProductName = c.Product.Name,
                UserId = c.UserId,
                UserName = $"{c.User.FirstName} {c.User.LastName}",
                Text = c.Text,
                StarCount = c.StarCount,
                IsConfirmed = c.IsConfirmed,
                CreatedAt = c.CreatedAt
            });
        }

        public Result Approve(int id)
        {
            var comment = _repository.GetById(id);
            if (comment == null) return Result.NotFound();

            comment.IsConfirmed = true;
            _repository.Update(comment);
            return Result.Success();
        }

        public Result Create(int userId, CommentCreateDto model)
        {
            var entity = new ProductCommentEntity
            {
                ProductId = model.ProductId,
                UserId = userId,
                Text = model.Text,
                StarCount = model.StarCount,
                IsConfirmed = false,
                CreatedAt = DateTime.Now
            };
            _repository.Add(entity);
            return Result.Success();
        }
    }
}