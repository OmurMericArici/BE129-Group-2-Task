using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;

namespace Shoppers.Web.AdminMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IRepository<UserEntity> _userRepository;

        public UserController(IRepository<UserEntity> userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult List()
        {
            var users = _userRepository.GetAll()
                                       .Include(u => u.Role)
                                       .OrderByDescending(u => u.CreatedAt)
                                       .ToList();
            return View(users);
        }

        [HttpGet]
        public IActionResult Approve(int id)
        {
            var user = _userRepository.GetAll()
                                      .Include(u => u.Role)
                                      .FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public IActionResult ApproveConfirmed(int id)
        {
            var user = _userRepository.GetById(id);
            if (user != null)
            {
                user.RoleId = 2;
                _userRepository.Update(user);
            }
            return RedirectToAction(nameof(List));
        }
    }
}