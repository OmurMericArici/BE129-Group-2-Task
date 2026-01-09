using Microsoft.AspNetCore.Mvc;
using Shoppers.Web.Mvc.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Shoppers.Data.Entities;

namespace Shoppers.Web.Mvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("DataApi");
            var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("auth/login", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = await JsonSerializer.DeserializeAsync<JsonElement>(responseStream, jsonOptions);

                if (result.TryGetProperty("Token", out var tokenProperty) || result.TryGetProperty("token", out tokenProperty))
                {
                    var token = tokenProperty.GetString();

                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                    if (roleClaim == "Admin")
                    {
                        ModelState.AddModelError("", "Yönetici hesabıyla buradan giriş yapamazsınız.");
                        return View(model);
                    }

                    Response.Cookies.Append("ShoppersToken", token!, new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = DateTime.Now.AddDays(7),
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Email veya şifre hatalı.");
            return View(model);
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("ShoppersToken");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userEntity = new UserEntity
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                RoleId = 1,
                Enabled = true,
                CreatedAt = DateTime.Now
            };

            var client = _httpClientFactory.CreateClient("DataApi");
            var jsonContent = new StringContent(JsonSerializer.Serialize(userEntity), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("auth/register", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "Kayıt başarılı! Lütfen giriş yapınız.";
                return View("Login");
            }

            var errorMsg = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Kayıt başarısız: {errorMsg}");
            return View(model);
        }
    }
}