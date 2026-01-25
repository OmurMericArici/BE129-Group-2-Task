using App.Models.DTO;
using App.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Shoppers.Web.AdminMvc.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Shoppers.Web.AdminMvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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

            var dto = new LoginRequestDto
            {
                Email = model.Email,
                Password = model.Password
            };

            var result = await _authService.LoginAsync(dto);

            if (result.IsSuccess)
            {
                var token = result.Value.Token;
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (roleClaim != "Admin")
                {
                    ModelState.AddModelError("", "Bu alana girmek için yetkiniz yok.");
                    return View(model);
                }

                Response.Cookies.Append("ShoppersAdminToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddDays(1),
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                return RedirectToAction("Index", "Home");
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