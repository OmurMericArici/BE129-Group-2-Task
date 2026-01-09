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
    public class OrderController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;

        public OrderController(IHttpClientFactory httpClientFactory)
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
        public async Task<IActionResult> Create()
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            var response = await client.GetAsync("cart");
            if (response.IsSuccessStatusCode)
            {
                var cartItems = await JsonSerializer.DeserializeAsync<List<CartItemEntity>>(await response.Content.ReadAsStreamAsync(), _jsonOptions);

                if (cartItems == null || !cartItems.Any())
                {
                    return RedirectToAction("Edit", "Cart");
                }

                ViewBag.CartItems = cartItems;
                ViewBag.TotalPrice = cartItems.Sum(c => c.Quantity * c.Product.Price);
                return View();
            }

            return RedirectToAction("Edit", "Cart");
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCartForView();
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            var jsonContent = new StringContent(JsonSerializer.Serialize(model.Address), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("order/checkout", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var result = await JsonSerializer.DeserializeAsync<JsonElement>(await response.Content.ReadAsStreamAsync());
                var orderId = result.GetProperty("orderId").GetInt32();
                return RedirectToAction("Details", new { id = orderId });
            }

            ModelState.AddModelError("", "Order could not be placed.");
            await LoadCartForView();
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            var response = await client.GetAsync("order/myorders");
            if (response.IsSuccessStatusCode)
            {
                var orders = await JsonSerializer.DeserializeAsync<List<OrderEntity>>(await response.Content.ReadAsStreamAsync(), _jsonOptions);
                var order = orders?.FirstOrDefault(o => o.Id == id);
                if (order != null) return View(order);
            }

            return NotFound();
        }

        private async Task LoadCartForView()
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);
            var response = await client.GetAsync("cart");
            if (response.IsSuccessStatusCode)
            {
                var cartItems = await JsonSerializer.DeserializeAsync<List<CartItemEntity>>(await response.Content.ReadAsStreamAsync(), _jsonOptions);
                ViewBag.CartItems = cartItems;
                ViewBag.TotalPrice = cartItems?.Sum(c => c.Quantity * c.Product.Price) ?? 0;
            }
        }
    }
}