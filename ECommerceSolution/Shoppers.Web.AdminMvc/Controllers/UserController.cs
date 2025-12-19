using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data;

namespace Shoppers.Web.AdminMvc.Controllers
{
    public class UserController : Controller
    {
        private readonly ShoppersDbContext _context;

        public UserController(ShoppersDbContext context)
        {
            _context = context;
        }

        public IActionResult List()
        {
            var users = _context.Users
                .Include(u => u.Role)
                .OrderByDescending(u => u.CreatedAt)
                .ToList();

            return View(users);
        }

        [HttpGet]
        public IActionResult Approve(int id)
        {
            var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public IActionResult ApproveConfirmed(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                // Kullanıcıyı "Seller" (Rol ID: 2) yapıyoruz
                // Not: Seed datada 1=Buyer, 2=Seller, 3=Admin olarak tanımlı.
                user.RoleId = 2;
                _context.Users.Update(user);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(List));
        }
    }
}