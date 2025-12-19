using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data;

namespace Shoppers.Web.AdminMvc.Controllers
{
    public class CommentController : Controller
    {
        private readonly ShoppersDbContext _context;

        public CommentController(ShoppersDbContext context)
        {
            _context = context;
        }

        public IActionResult List()
        {
            var comments = _context.ProductComments
                .Include(c => c.Product)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            return View(comments);
        }

        [HttpGet]
        public IActionResult Approve(int id)
        {
            var comment = _context.ProductComments
                .Include(c => c.Product)
                .Include(c => c.User)
                .FirstOrDefault(c => c.Id == id);

            if (comment == null) return NotFound();

            return View(comment);
        }

        [HttpPost]
        public IActionResult ApproveConfirmed(int id)
        {
            var comment = _context.ProductComments.Find(id);
            if (comment != null)
            {
                comment.IsConfirmed = true;
                _context.ProductComments.Update(comment);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(List));
        }
    }
}