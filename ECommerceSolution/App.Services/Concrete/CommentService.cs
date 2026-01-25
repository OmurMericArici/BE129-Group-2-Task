using App.Models.DTO;
using App.Services.Abstract;
using Ardalis.Result;

namespace App.Services.Concrete
{
    public class CommentService : BaseService, ICommentService
    {
        public CommentService(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public async Task<Result<List<CommentDto>>> GetAllAsync(string jwt)
        {
            return await SendRequestAsync<List<CommentDto>>("comment", HttpMethod.Get, jwt);
        }

        public async Task<Result<CommentDto>> GetByIdAsync(string jwt, int id)
        {
            return await SendRequestAsync<CommentDto>($"comment/{id}", HttpMethod.Get, jwt);
        }

        public async Task<Result> ApproveAsync(string jwt, int id)
        {
            return await SendRequestAsync($"comment/approve/{id}", HttpMethod.Post, jwt);
        }

        public async Task<Result> CreateAsync(string jwt, CommentCreateDto model)
        {
            return await SendRequestAsync("comment", HttpMethod.Post, jwt, model);
        }
    }
}