using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;

namespace Shoppers.Web.AdminMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CommentController : Controller
    {
        private readonly IRepository<ProductCommentEntity> _commentRepository;

        public CommentController(IRepository<ProductCommentEntity> commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public IActionResult List()
        {
            var comments = _commentRepository.GetAll()
                                             .Include(c => c.Product)
                                             .Include(c => c.User)
                                             .OrderByDescending(c => c.CreatedAt)
                                             .ToList();
            return View(comments);
        }

        [HttpGet]
        public IActionResult Approve(int id)
        {
            var comment = _commentRepository.GetAll()
                                            .Include(c => c.Product)
                                            .Include(c => c.User)
                                            .FirstOrDefault(c => c.Id == id);

            if (comment == null) return NotFound();

            return View(comment);
        }

        [HttpPost]
        public IActionResult ApproveConfirmed(int id)
        {
            var comment = _commentRepository.GetById(id);
            if (comment != null)
            {
                comment.IsConfirmed = true;
                _commentRepository.Update(comment);
            }
            return RedirectToAction(nameof(List));
        }
    }
}