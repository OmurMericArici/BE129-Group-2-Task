using App.Models.DTO;
using Ardalis.Result;

namespace App.Services.Abstract
{
    public interface IUserService
    {
        Task<Result<UserDto>> GetMeAsync(string jwt);
        Task<Result> UpdateMeAsync(string jwt, UserUpdateDto model);
        Task<Result<List<UserDto>>> GetAllAsync(string jwt);
        Task<Result<UserDto>> GetByIdAsync(string jwt, int id);
        Task<Result> ApproveSellerAsync(string jwt, int id);
    }
}