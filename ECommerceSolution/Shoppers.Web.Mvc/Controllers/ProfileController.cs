using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data;
using Shoppers.Web.Mvc.Models;
using System.Security.Claims;

namespace Shoppers.Web.Mvc.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ShoppersDbContext _context;

        public ProfileController(ShoppersDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        [HttpGet]
        public IActionResult Details()
        {
            var userId = GetUserId();
            var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == userId);

            if (user == null) return NotFound();

            return View(user);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var userId = GetUserId();
            var user = _context.Users.Find(userId);

            if (user == null) return NotFound();

            var model = new ProfileEditViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(ProfileEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = GetUserId();
            var user = _context.Users.Find(userId);

            if (user == null) return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            _context.Users.Update(user);
            _context.SaveChanges();

            ViewBag.SuccessMessage = "Profil bilgileriniz güncellendi.";
            return View(model);
        }

        public IActionResult MyOrders()
        {
            return View();
        }

        public IActionResult MyProducts()
        {
            var userId = GetUserId();

            var products = _context.Products
                                   .Include(p => p.Images)
                                   .Where(p => p.SellerId == userId && p.Enabled)
                                   .OrderByDescending(p => p.CreatedAt)
                                   .ToList();

            return View(products);
        }
    }
}