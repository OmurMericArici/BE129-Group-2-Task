using App.Models.DTO;
using App.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Shoppers.Web.Mvc.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Shoppers.Web.Mvc.Controllers
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

            var loginDto = new LoginRequestDto
            {
                Email = model.Email,
                Password = model.Password
            };

            var result = await _authService.LoginAsync(loginDto);

            if (result.IsSuccess)
            {
                var token = result.Value.Token;
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (roleClaim == "Admin")
                {
                    ModelState.AddModelError("", "Yönetici hesabıyla buradan giriş yapamazsınız.");
                    return View(model);
                }

                Response.Cookies.Append("ShoppersToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddDays(7),
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                return RedirectToAction("Index", "Home");
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

            var registerDto = new RegisterRequestDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword
            };

            var result = await _authService.RegisterAsync(registerDto);

            if (result.IsSuccess)
            {
                ViewBag.SuccessMessage = "Kayıt başarılı! Lütfen giriş yapınız.";
                return View("Login");
            }

            ModelState.AddModelError("", string.Join(", ", result.Errors));
            return View(model);
        }
    }
}