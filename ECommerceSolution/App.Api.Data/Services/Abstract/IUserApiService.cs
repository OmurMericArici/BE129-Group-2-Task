using App.Models.DTO;
using Ardalis.Result;

namespace App.Api.Data.Services.Abstract
{
    public interface IUserApiService
    {
        Result<UserDto> GetUser(int userId);
        Result<UserDto> UpdateUser(int userId, UserUpdateDto model);
        Result<List<UserDto>> GetAllUsers();
        Result ApproveSeller(int userId);
    }
}