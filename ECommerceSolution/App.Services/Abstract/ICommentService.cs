using App.Models.DTO;
using Ardalis.Result;

namespace App.Services.Abstract
{
    public interface ICommentService
    {
        Task<Result<List<CommentDto>>> GetAllAsync(string jwt);
        Task<Result<CommentDto>> GetByIdAsync(string jwt, int id);
        Task<Result> ApproveAsync(string jwt, int id);
        Task<Result> CreateAsync(string jwt, CommentCreateDto model);
    }
}