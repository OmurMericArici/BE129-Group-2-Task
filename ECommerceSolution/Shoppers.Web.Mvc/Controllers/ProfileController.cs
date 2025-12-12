using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data;

namespace Shoppers.Web.Mvc.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ShoppersDbContext _context;

        public ProfileController(ShoppersDbContext context)
        {
            _context = context;
        }

        public IActionResult Details()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult MyOrders()
        {
            return View();
        }

        public IActionResult MyProducts()
        {
            var userId = 1;

            var products = _context.Products
                                   .Include(p => p.Images)
                                   .Where(p => p.SellerId == userId && p.Enabled)
                                   .OrderByDescending(p => p.CreatedAt)
                                   .ToList();

            return View(products);
        }
    }
}