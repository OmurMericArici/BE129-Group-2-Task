using App.Models.DTO;
using Ardalis.Result;

namespace App.Api.Data.Services.Abstract
{
    public interface IAuthApiService
    {
        Result<LoginResponseDto> Login(LoginRequestDto model);
        Result Register(RegisterRequestDto model);
    }
}