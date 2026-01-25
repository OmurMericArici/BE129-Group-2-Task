using App.Api.Data.Services.Abstract;
using App.Models.DTO;
using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;

namespace App.Api.Data.Services.Concrete
{
    public class UserApiService : IUserApiService
    {
        private readonly IRepository<UserEntity> _repository;

        public UserApiService(IRepository<UserEntity> repository)
        {
            _repository = repository;
        }

        public Result<UserDto> GetUser(int userId)
        {
            var user = _repository.GetAll().Include(u => u.Role).FirstOrDefault(u => u.Id == userId);
            if (user == null) return Result.NotFound();

            return Result.Success(new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.Name,
                CreatedAt = user.CreatedAt
            });
        }

        public Result<UserDto> UpdateUser(int userId, UserUpdateDto model)
        {
            var user = _repository.GetById(userId);
            if (user == null) return Result.NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            _repository.Update(user);
            return GetUser(userId);
        }

        public Result<List<UserDto>> GetAllUsers()
        {
            var users = _repository.GetAll()
                .Include(u => u.Role)
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Role = u.Role.Name,
                    CreatedAt = u.CreatedAt
                })
                .ToList();
            return Result.Success(users);
        }

        public Result ApproveSeller(int userId)
        {
            var user = _repository.GetById(userId);
            if (user == null) return Result.NotFound();

            user.RoleId = 2; // Seller
            _repository.Update(user);
            return Result.Success();
        }
    }
}