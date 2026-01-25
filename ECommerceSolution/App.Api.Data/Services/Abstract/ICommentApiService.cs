using App.Models.DTO;
using Ardalis.Result;

namespace App.Api.Data.Services.Abstract
{
    public interface ICommentApiService
    {
        Result<List<CommentDto>> GetAll();
        Result<CommentDto> GetById(int id);
        Result Approve(int id);
        Result Create(int userId, CommentCreateDto model);
    }
}