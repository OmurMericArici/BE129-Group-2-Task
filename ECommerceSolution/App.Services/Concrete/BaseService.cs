using Ardalis.Result;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace App.Services.Concrete
{
    public abstract class BaseService
    {
        protected readonly HttpClient _client;
        protected readonly JsonSerializerOptions _jsonOptions;

        public BaseService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("DataApi");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        protected async Task<Result<T>> SendRequestAsync<T>(string endpoint, HttpMethod method, string? jwt = null, object? payload = null)
        {
            var request = new HttpRequestMessage(method, endpoint);

            if (!string.IsNullOrEmpty(jwt))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            }

            if (payload != null)
            {
                request.Content = JsonContent.Create(payload);
            }

            try
            {
                var response = await _client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) return Result.Unauthorized();
                    if (response.StatusCode == System.Net.HttpStatusCode.Forbidden) return Result.Forbidden();
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return Result.NotFound();

                    return Result.Error(response.ReasonPhrase);
                }

                if (typeof(T) == typeof(bool))
                {
                    return Result.Success((T)(object)true);
                }

                var data = await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
                return Result.Success(data!);
            }
            catch (Exception ex)
            {
                return Result.Error(ex.Message);
            }
        }

        protected async Task<Result> SendRequestAsync(string endpoint, HttpMethod method, string? jwt = null, object? payload = null)
        {
            var request = new HttpRequestMessage(method, endpoint);

            if (!string.IsNullOrEmpty(jwt))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            }

            if (payload != null)
            {
                request.Content = JsonContent.Create(payload);
            }

            try
            {
                var response = await _client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) return Result.Unauthorized();
                    if (response.StatusCode == System.Net.HttpStatusCode.Forbidden) return Result.Forbidden();
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return Result.NotFound();

                    return Result.Error(response.ReasonPhrase);
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Error(ex.Message);
            }
        }
    }
}