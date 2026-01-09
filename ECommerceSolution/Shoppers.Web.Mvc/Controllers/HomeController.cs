using Microsoft.AspNetCore.Mvc;
using Shoppers.Data.Entities;
using System.Text.Json;

namespace Shoppers.Web.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("DataApi");

            var response = await client.GetAsync("product");

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var products = await JsonSerializer.DeserializeAsync<List<ProductEntity>>(stream, _jsonOptions);

                var featured = products?.Take(6).ToList();
                return View(featured);
            }

            return View(new List<ProductEntity>());
        }

        public async Task<IActionResult> Listing(string category, string sort)
        {
            var client = _httpClientFactory.CreateClient("DataApi");

            var productResponse = await client.GetAsync("product");
            var products = new List<ProductEntity>();
            if (productResponse.IsSuccessStatusCode)
            {
                var stream = await productResponse.Content.ReadAsStreamAsync();
                products = await JsonSerializer.DeserializeAsync<List<ProductEntity>>(stream, _jsonOptions);
            }

            var categoryResponse = await client.GetAsync("category");
            var categories = new List<CategoryEntity>();
            if (categoryResponse.IsSuccessStatusCode)
            {
                var stream = await categoryResponse.Content.ReadAsStreamAsync();
                categories = await JsonSerializer.DeserializeAsync<List<CategoryEntity>>(stream, _jsonOptions);
            }

            var query = products!.AsQueryable();

            if (!string.IsNullOrEmpty(category) && category != "all")
            {
                query = query.Where(p => p.Category != null && p.Category.Name == category);
            }

            switch (sort)
            {
                case "newest": query = query.OrderByDescending(p => p.CreatedAt); break;
                case "price_asc": query = query.OrderBy(p => p.Price); break;
                case "price_desc": query = query.OrderByDescending(p => p.Price); break;
                default: query = query.OrderByDescending(p => p.CreatedAt); break;
            }

            ViewBag.Categories = categories;
            ViewBag.CurrentCategory = category;

            return View(query.ToList());
        }

        public async Task<IActionResult> ProductDetail(int id)
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            var response = await client.GetAsync($"product/{id}");

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var product = await JsonSerializer.DeserializeAsync<ProductEntity>(stream, _jsonOptions);
                return View(product);
            }

            return NotFound();
        }

        public IActionResult AboutUs() => View();
        public IActionResult Contact() => View();
    }
}