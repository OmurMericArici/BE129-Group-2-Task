using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shoppers.Data.Entities;
using Shoppers.Web.Mvc.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Shoppers.Web.Mvc.Controllers
{
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IConfiguration _configuration;

        public ProductController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
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

        private async Task PrepareCategoriesViewBag()
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            var response = await client.GetAsync("category");
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var categories = await JsonSerializer.DeserializeAsync<List<CategoryEntity>>(stream, _jsonOptions);
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
            }
        }

        [Authorize(Roles = "Seller")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareCategoriesViewBag();
            return View();
        }

        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PrepareCategoriesViewBag();
                return View(model);
            }

            string? uploadedFileName = null;
            if (model.ImageFile != null)
            {
                var fileClient = _httpClientFactory.CreateClient("FileApi");
                using var content = new MultipartFormDataContent();
                using var fileStream = model.ImageFile.OpenReadStream();
                content.Add(new StreamContent(fileStream), "file", model.ImageFile.FileName);

                var fileResponse = await fileClient.PostAsync("file/upload", content);
                if (fileResponse.IsSuccessStatusCode)
                {
                    var fileResult = await JsonSerializer.DeserializeAsync<JsonElement>(await fileResponse.Content.ReadAsStreamAsync());
                    uploadedFileName = fileResult.GetProperty("fileName").GetString();
                }
            }

            var product = new ProductEntity
            {
                Name = model.Name,
                Price = model.Price,
                StockAmount = (byte)model.StockAmount,
                CategoryId = model.CategoryId,
                Details = model.Details,
                Images = new List<ProductImageEntity>()
            };

            if (uploadedFileName != null)
            {
                var fileApiUrl = _configuration["ApiSettings:FileApiUrl"];
                var fullUrl = $"{fileApiUrl}file/download?fileName={uploadedFileName}";

                product.Images.Add(new ProductImageEntity
                {
                    Url = fullUrl,
                    CreatedAt = DateTime.Now
                });
            }

            var dataClient = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(dataClient);

            var jsonContent = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var response = await dataClient.PostAsync("product", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("MyProducts", "Profile");
            }

            ModelState.AddModelError("", "Ürün eklenirken hata oluştu.");
            await PrepareCategoriesViewBag();
            return View(model);
        }

        [Authorize(Roles = "Seller")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            var response = await client.GetAsync($"product/{id}");

            if (!response.IsSuccessStatusCode) return NotFound();

            var stream = await response.Content.ReadAsStreamAsync();
            var product = await JsonSerializer.DeserializeAsync<ProductEntity>(stream, _jsonOptions);

            var model = new ProductEditViewModel
            {
                Id = product!.Id,
                Name = product.Name,
                Price = product.Price,
                StockAmount = product.StockAmount,
                Details = product.Details,
                CategoryId = product.CategoryId,
                CurrentImageUrl = product.Images?.FirstOrDefault()?.Url
            };

            await PrepareCategoriesViewBag();
            return View(model);
        }

        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PrepareCategoriesViewBag();
                return View(model);
            }

            string? uploadedFileName = null;
            if (model.ImageFile != null)
            {
                var fileClient = _httpClientFactory.CreateClient("FileApi");
                using var content = new MultipartFormDataContent();
                using var fileStream = model.ImageFile.OpenReadStream();
                content.Add(new StreamContent(fileStream), "file", model.ImageFile.FileName);

                var fileResponse = await fileClient.PostAsync("file/upload", content);
                if (fileResponse.IsSuccessStatusCode)
                {
                    var fileResult = await JsonSerializer.DeserializeAsync<JsonElement>(await fileResponse.Content.ReadAsStreamAsync());
                    uploadedFileName = fileResult.GetProperty("fileName").GetString();
                }
            }

            var dataClient = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(dataClient);

            var existingResponse = await dataClient.GetAsync($"product/{model.Id}");
            if (!existingResponse.IsSuccessStatusCode) return NotFound();
            var existingProduct = await JsonSerializer.DeserializeAsync<ProductEntity>(await existingResponse.Content.ReadAsStreamAsync(), _jsonOptions);

            existingProduct!.Name = model.Name;
            existingProduct.Price = model.Price;
            existingProduct.StockAmount = (byte)model.StockAmount;
            existingProduct.Details = model.Details;
            existingProduct.CategoryId = model.CategoryId;

            if (uploadedFileName != null)
            {
                var fileApiUrl = _configuration["ApiSettings:FileApiUrl"];
                var fullUrl = $"{fileApiUrl}file/download?fileName={uploadedFileName}";

                if (existingProduct.Images == null) existingProduct.Images = new List<ProductImageEntity>();

                var currentImg = existingProduct.Images.FirstOrDefault();
                if (currentImg != null)
                {
                    currentImg.Url = fullUrl;
                }
                else
                {
                    existingProduct.Images.Add(new ProductImageEntity { Url = fullUrl, CreatedAt = DateTime.Now });
                }
            }

            var jsonContent = new StringContent(JsonSerializer.Serialize(existingProduct), Encoding.UTF8, "application/json");
            var response = await dataClient.PutAsync("product", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("MyProducts", "Profile");
            }

            return View(model);
        }

        [Authorize(Roles = "Seller")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
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

        [Authorize(Roles = "Seller")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            AddAuthHeader(client);

            var response = await client.DeleteAsync($"product/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("MyProducts", "Profile");
            }
            return View();
        }

        [Authorize(Roles = "Buyer,Seller")]
        [HttpGet]
        public async Task<IActionResult> Comment(int id)
        {
            var client = _httpClientFactory.CreateClient("DataApi");
            var response = await client.GetAsync($"product/{id}");

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var product = await JsonSerializer.DeserializeAsync<ProductEntity>(stream, _jsonOptions);

                ViewBag.ProductName = product!.Name;
                ViewBag.ProductImage = product.Images?.FirstOrDefault()?.Url;
                ViewBag.Price = product.Price;

                return View(new ProductCommentViewModel { ProductId = id });
            }
            return NotFound();
        }

        [Authorize(Roles = "Buyer,Seller")]
        [HttpPost]
        public IActionResult Comment(ProductCommentViewModel model)
        {
            return RedirectToAction("Listing", "Home");
        }
    }
}