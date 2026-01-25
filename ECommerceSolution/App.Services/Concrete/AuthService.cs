using App.Models.DTO;
using App.Services.Abstract;
using Ardalis.Result;

namespace App.Services.Concrete
{
    public class AuthService : BaseService, IAuthService
    {
        public AuthService(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto model)
        {
            return await SendRequestAsync<LoginResponseDto>("auth/login", HttpMethod.Post, null, model);
        }

        public async Task<Result> RegisterAsync(RegisterRequestDto model)
        {
            return await SendRequestAsync("auth/register", HttpMethod.Post, null, model);
        }
    }
}