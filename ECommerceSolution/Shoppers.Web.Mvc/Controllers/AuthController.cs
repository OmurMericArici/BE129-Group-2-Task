using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Shoppers.Data;
using Shoppers.Data.Entities;
using Shoppers.Web.Mvc.Models;
using System.Security.Claims;

namespace Shoppers.Web.Mvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly ShoppersDbContext _context;

        public AuthController(ShoppersDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Bu email adresi zaten kullanımda.");
                return View(model);
            }

            var newUser = new UserEntity
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                RoleId = 1,
                CreatedAt = DateTime.Now,
                Enabled = true
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            ViewBag.SuccessMessage = "Kayıt başarılı! Lütfen giriş yapınız.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Email veya şifre hatalı.");
                return View(model);
            }

            if (!user.Enabled)
            {
                ModelState.AddModelError("", "Hesabınız pasif durumdadır.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("FullName", $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.RoleId == 3 ? "Admin" : (user.RoleId == 2 ? "Seller" : "Buyer"))
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}