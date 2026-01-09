using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppers.Data.Entities;
using Shoppers.Web.Mvc.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Shoppers.Web.Mvc.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;

        public CartController(IHttpClientFactory httpClientFactory)
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

        [HttpPost]
        public async Task<IActionResult> AddProduct(int productId, int quantity = 1)
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            var response = await client.PostAsync($"cart/add?productId={productId}&quantity={quantity}", null);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not add item to cart.";
            }

            return RedirectToAction(nameof(Edit));
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            var response = await client.GetAsync("cart");
            var cartItems = new List<CartItemEntity>();

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                cartItems = await JsonSerializer.DeserializeAsync<List<CartItemEntity>>(stream, _jsonOptions);
            }

            var viewModel = new CartViewModel
            {
                Items = cartItems!.Select(c => new CartItemViewModel
                {
                    CartItemId = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    Price = c.Product.Price,
                    Quantity = c.Quantity,
                    StockAvailable = c.Product.StockAmount,
                    ImageUrl = c.Product.Images.FirstOrDefault()?.Url
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(Dictionary<int, int> quantities)
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            var jsonContent = new StringContent(JsonSerializer.Serialize(quantities), Encoding.UTF8, "application/json");
            await client.PutAsync("cart/update", jsonContent);

            return RedirectToAction(nameof(Edit));
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            await client.DeleteAsync($"cart/{cartItemId}");

            return RedirectToAction(nameof(Edit));
        }
    }
}