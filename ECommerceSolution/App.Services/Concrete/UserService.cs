using App.Models.DTO;
using App.Services.Abstract;
using Ardalis.Result;

namespace App.Services.Concrete
{
    public class UserService : BaseService, IUserService
    {
        public UserService(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public async Task<Result<UserDto>> GetMeAsync(string jwt)
        {
            return await SendRequestAsync<UserDto>("user/me", HttpMethod.Get, jwt);
        }

        public async Task<Result> UpdateMeAsync(string jwt, UserUpdateDto model)
        {
            return await SendRequestAsync("user/me", HttpMethod.Put, jwt, model);
        }

        public async Task<Result<List<UserDto>>> GetAllAsync(string jwt)
        {
            return await SendRequestAsync<List<UserDto>>("user", HttpMethod.Get, jwt);
        }

        public async Task<Result<UserDto>> GetByIdAsync(string jwt, int id)
        {
            return await SendRequestAsync<UserDto>($"user/{id}", HttpMethod.Get, jwt);
        }

        public async Task<Result> ApproveSellerAsync(string jwt, int id)
        {
            return await SendRequestAsync($"user/approve/{id}", HttpMethod.Post, jwt);
        }
    }
}