using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppers.Data.Entities;
using Shoppers.Web.Mvc.Models;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace Shoppers.Web.Mvc.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProfileController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        private void AddAuthHeader(HttpClient client)
        {
            var token = Request.Cookies["ShoppersToken"];
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            var response = await client.GetAsync("user/me");
            if (response.IsSuccessStatusCode)
            {
                var user = await JsonSerializer.DeserializeAsync<UserEntity>(await response.Content.ReadAsStreamAsync(), _jsonOptions);
                return View(user);
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            var response = await client.GetAsync("user/me");
            if (response.IsSuccessStatusCode)
            {
                var user = await JsonSerializer.DeserializeAsync<UserEntity>(await response.Content.ReadAsStreamAsync(), _jsonOptions);
                var model = new ProfileEditViewModel
                {
                    FirstName = user!.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                };
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            var userEntity = new UserEntity { FirstName = model.FirstName, LastName = model.LastName };
            var content = new StringContent(JsonSerializer.Serialize(userEntity), System.Text.Encoding.UTF8, "application/json");

            var response = await client.PutAsync("user/me", content);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "Profile updated.";
                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            var response = await client.GetAsync("order/myorders");
            if (response.IsSuccessStatusCode)
            {
                var orders = await JsonSerializer.DeserializeAsync<List<OrderEntity>>(await response.Content.ReadAsStreamAsync(), _jsonOptions);
                return View(orders);
            }
            return View(new List<OrderEntity>());
        }

        [Authorize(Roles = "Seller")]
        [HttpGet]
        public async Task<IActionResult> MyProducts()
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await client.GetAsync($"product/seller/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var products = await JsonSerializer.DeserializeAsync<List<ProductEntity>>(await response.Content.ReadAsStreamAsync(), _jsonOptions);
                return View(products);
            }
            return View(new List<ProductEntity>());
        }
    }
}