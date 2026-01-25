using App.Models.DTO;
using App.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shoppers.Web.AdminMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CommentController : Controller
    {
        private readonly ICommentService _service;

        public CommentController(ICommentService service)
        {
            _service = service;
        }

        private string GetJwt() => Request.Cookies["ShoppersAdminToken"]!;

        public async Task<IActionResult> List()
        {
            var result = await _service.GetAllAsync(GetJwt());
            if (result.IsSuccess) return View(result.Value);
            return View(new List<CommentDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Approve(int id)
        {
            var result = await _service.GetByIdAsync(GetJwt(), id);
            if (result.IsSuccess) return View(result.Value);
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ApproveConfirmed(int id)
        {
            await _service.ApproveAsync(GetJwt(), id);
            return RedirectToAction(nameof(List));
        }
    }
}