using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CommentController : ControllerBase
    {
        private readonly IRepository<ProductCommentEntity> _commentRepository;

        public CommentController(IRepository<ProductCommentEntity> commentRepository)
        {
            _commentRepository = commentRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var comments = _commentRepository.GetAll()
                .Include(c => c.Product)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
            return Ok(comments);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var comment = _commentRepository.GetAll()
                .Include(c => c.Product)
                .Include(c => c.User)
                .FirstOrDefault(c => c.Id == id);

            if (comment == null) return NotFound();
            return Ok(comment);
        }

        [HttpPost("approve/{id}")]
        public IActionResult Approve(int id)
        {
            var comment = _commentRepository.GetById(id);
            if (comment == null) return NotFound();

            comment.IsConfirmed = true;
            _commentRepository.Update(comment);
            return Ok();
        }
    }
}