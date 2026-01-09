using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppers.Data.Entities;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Shoppers.Web.AdminMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProductController(IHttpClientFactory httpClientFactory)
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

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            var response = await client.GetAsync("product");

            if (response.IsSuccessStatusCode)
            {
                var products = await JsonSerializer.DeserializeAsync<List<ProductEntity>>(await response.Content.ReadAsStreamAsync(), _jsonOptions);
                return View(products);
            }
            return View(new List<ProductEntity>());
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            await client.DeleteAsync($"product/{id}");
            return RedirectToAction("Index");
        }
    }
}