using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppers.Data.Entities;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Shoppers.Web.AdminMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CommentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;

        public CommentController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        private void AddAuthHeader(HttpClient client)
        {
            var token = Request.Cookies["ShoppersAdminToken"];
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<IActionResult> List()
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            var response = await client.GetAsync("comment");
            if (response.IsSuccessStatusCode)
            {
                var comments = await JsonSerializer.DeserializeAsync<List<ProductCommentEntity>>(await response.Content.ReadAsStreamAsync(), _jsonOptions);
                return View(comments);
            }
            return View(new List<ProductCommentEntity>());
        }

        [HttpGet]
        public async Task<IActionResult> Approve(int id)
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            var response = await client.GetAsync($"comment/{id}");
            if (response.IsSuccessStatusCode)
            {
                var comment = await JsonSerializer.DeserializeAsync<ProductCommentEntity>(await response.Content.ReadAsStreamAsync(), _jsonOptions);
                return View(comment);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ApproveConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            await client.PostAsync($"comment/approve/{id}", null);
            return RedirectToAction(nameof(List));
        }
    }
}