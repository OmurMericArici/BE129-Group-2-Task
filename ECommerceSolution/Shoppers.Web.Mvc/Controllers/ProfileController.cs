using App.Models.DTO;
using App.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppers.Web.Mvc.Models;
using System.Security.Claims;

namespace Shoppers.Web.Mvc.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        public ProfileController(IUserService userService, IOrderService orderService, IProductService productService)
        {
            _userService = userService;
            _orderService = orderService;
            _productService = productService;
        }

        private string GetJwt() => Request.Cookies["ShoppersToken"]!;

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var result = await _userService.GetMeAsync(GetJwt());
            if (result.IsSuccess) return View(result.Value);
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var result = await _userService.GetMeAsync(GetJwt());
            if (result.IsSuccess)
            {
                var user = result.Value;
                var model = new ProfileEditViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                };
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = new UserUpdateDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userService.UpdateMeAsync(GetJwt(), dto);

            if (result.IsSuccess)
            {
                ViewBag.SuccessMessage = "Profile updated.";
                return View(model);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var result = await _orderService.GetMyOrdersAsync(GetJwt());
            if (result.IsSuccess) return View(result.Value);
            return View(new List<OrderDto>());
        }

        [Authorize(Roles = "Seller")]
        [HttpGet]
        public async Task<IActionResult> MyProducts()
        {
            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _productService.GetMyProductsAsync(GetJwt(), sellerId);
            if (result.IsSuccess) return View(result.Value);
            return View(new List<ProductDto>());
        }
    }
}