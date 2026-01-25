using Ardalis.Result;
using Microsoft.AspNetCore.Http;

namespace App.Services.Abstract
{
    public interface IFileService
    {
        Task<Result<string>> UploadFileAsync(IFormFile file);
    }
}