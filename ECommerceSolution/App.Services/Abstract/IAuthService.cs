using App.Models.DTO;
using Ardalis.Result;

namespace App.Services.Abstract
{
    public interface IAuthService
    {
        Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto model);
        Task<Result> RegisterAsync(RegisterRequestDto model);
    }
}