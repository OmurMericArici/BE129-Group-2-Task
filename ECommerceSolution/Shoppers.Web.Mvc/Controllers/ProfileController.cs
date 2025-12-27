using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;
using Shoppers.Web.Mvc.Models;
using System.Security.Claims;

namespace Shoppers.Web.Mvc.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IRepository<ProductEntity> _productRepository;
        private readonly IRepository<OrderEntity> _orderRepository;

        public ProfileController(
            IRepository<UserEntity> userRepository,
            IRepository<ProductEntity> productRepository,
            IRepository<OrderEntity> orderRepository)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
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
            var user = _userRepository.GetAll()
                                      .Include(u => u.Role)
                                      .FirstOrDefault(u => u.Id == userId);

            if (user == null) return NotFound();

            return View(user);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var userId = GetUserId();
            var user = _userRepository.GetById(userId);

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
            var user = _userRepository.GetById(userId);

            if (user == null) return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            _userRepository.Update(user);

            ViewBag.SuccessMessage = "Profil bilgileriniz güncellendi.";
            return View(model);
        }

        [Authorize(Roles = "Buyer,Seller")]
        public IActionResult MyOrders()
        {
            var userId = GetUserId();
            var orders = _orderRepository.GetAll()
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return View(orders);
        }

        [Authorize(Roles = "Seller")]
        public IActionResult MyProducts()
        {
            var userId = GetUserId();

            var products = _productRepository.GetAll()
                                   .Include(p => p.Images)
                                   .Where(p => p.SellerId == userId && p.Enabled)
                                   .OrderByDescending(p => p.CreatedAt)
                                   .ToList();

            return View(products);
        }
    }
}