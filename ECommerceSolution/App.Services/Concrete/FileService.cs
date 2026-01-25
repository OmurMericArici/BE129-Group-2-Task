using App.Services.Abstract;
using Ardalis.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace App.Services.Concrete
{
    public class FileService : IFileService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public FileService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _client = httpClientFactory.CreateClient("FileApi");
            _configuration = configuration;
        }

        public async Task<Result<string>> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return Result.Error("No file");

            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            content.Add(new StreamContent(stream), "file", file.FileName);

            var response = await _client.PostAsync("file/upload", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                var fileName = json.GetProperty("fileName").GetString();

                var fileApiUrl = _configuration["ApiSettings:FileApiUrl"];
                var fullUrl = $"{fileApiUrl}file/download?fileName={fileName}";

                return Result.Success(fullUrl);
            }

            return Result.Error("Upload failed");
        }
    }
}