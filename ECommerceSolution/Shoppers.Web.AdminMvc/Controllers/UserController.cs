using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppers.Data.Entities;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Shoppers.Web.AdminMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;

        public UserController(IHttpClientFactory httpClientFactory)
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

            var response = await client.GetAsync("user");
            if (response.IsSuccessStatusCode)
            {
                var users = await JsonSerializer.DeserializeAsync<List<UserEntity>>(await response.Content.ReadAsStreamAsync(), _jsonOptions);
                return View(users);
            }
            return View(new List<UserEntity>());
        }

        [HttpGet]
        public async Task<IActionResult> Approve(int id)
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            var response = await client.GetAsync($"user/{id}");
            if (response.IsSuccessStatusCode)
            {
                var user = await JsonSerializer.DeserializeAsync<UserEntity>(await response.Content.ReadAsStreamAsync(), _jsonOptions);
                return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ApproveConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            await client.PostAsync($"user/approve/{id}", null);
            return RedirectToAction(nameof(List));
        }
    }
}