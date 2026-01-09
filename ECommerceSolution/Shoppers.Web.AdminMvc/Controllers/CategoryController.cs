using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppers.Data.Entities;
using Shoppers.Web.AdminMvc.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Shoppers.Web.AdminMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;

        public CategoryController(IHttpClientFactory httpClientFactory)
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
            var response = await client.GetAsync("category");

            if (response.IsSuccessStatusCode)
            {
                var categories = await JsonSerializer.DeserializeAsync<List<CategoryEntity>>(await response.Content.ReadAsStreamAsync(), _jsonOptions);
                return View(categories);
            }
            return View(new List<CategoryEntity>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var category = new CategoryEntity
            {
                Name = model.Name,
                Color = model.Color,
                IconCssClass = model.IconCssClass
            };

            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            var jsonContent = new StringContent(JsonSerializer.Serialize(category), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("category", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }

            ModelState.AddModelError("", "Error creating category.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            var response = await client.GetAsync($"category/{id}");

            if (response.IsSuccessStatusCode)
            {
                var category = await JsonSerializer.DeserializeAsync<CategoryEntity>(await response.Content.ReadAsStreamAsync(), _jsonOptions);
                var model = new CategoryEditViewModel
                {
                    Id = category!.Id,
                    Name = category.Name,
                    Color = category.Color,
                    IconCssClass = category.IconCssClass
                };
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var category = new CategoryEntity
            {
                Id = model.Id,
                Name = model.Name,
                Color = model.Color,
                IconCssClass = model.IconCssClass
            };

            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            var jsonContent = new StringContent(JsonSerializer.Serialize(category), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("category", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            var response = await client.GetAsync($"category/{id}");

            if (response.IsSuccessStatusCode)
            {
                var category = await JsonSerializer.DeserializeAsync<CategoryEntity>(await response.Content.ReadAsStreamAsync(), _jsonOptions);
                return View(category);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            await client.DeleteAsync($"category/{id}");
            return RedirectToAction("List");
        }
    }
}