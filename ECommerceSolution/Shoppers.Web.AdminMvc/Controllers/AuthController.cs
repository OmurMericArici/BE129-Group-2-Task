using Microsoft.AspNetCore.Mvc;
using Shoppers.Web.AdminMvc.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Shoppers.Web.AdminMvc.Controllers
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

                    if (roleClaim != "Admin")
                    {
                        ModelState.AddModelError("", "Bu alana girmek için yetkiniz yok.");
                        return View(model);
                    }

                    Response.Cookies.Append("ShoppersAdminToken", token!, new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = DateTime.Now.AddDays(1),
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Giriş başarısız veya yetkisiz.");
            return View(model);
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("ShoppersAdminToken");
            return RedirectToAction("Login");
        }
    }
}