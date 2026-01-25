using App.Models.DTO;
using App.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppers.Web.Mvc.Models;

namespace Shoppers.Web.Mvc.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        private string GetJwt() => Request.Cookies["ShoppersToken"]!;

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var result = await _cartService.GetCartAsync(GetJwt());
            if (result.IsSuccess && result.Value.Any())
            {
                var cartItems = result.Value;
                ViewBag.CartItems = cartItems;
                ViewBag.TotalPrice = cartItems.Sum(c => c.Quantity * c.Price);
                return View();
            }
            return RedirectToAction("Edit", "Cart");
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCartForView();
                return View(model);
            }

            var dto = new OrderCreateRequestDto { Address = model.Address };
            var result = await _orderService.CreateOrderAsync(GetJwt(), dto);

            if (result.IsSuccess)
            {
                return RedirectToAction("Details", new { id = result.Value.OrderId });
            }

            ModelState.AddModelError("", "Order could not be placed.");
            await LoadCartForView();
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _orderService.GetMyOrdersAsync(GetJwt());
            if (result.IsSuccess)
            {
                var order = result.Value.FirstOrDefault(o => o.Id == id);
                if (order != null) return View(order);
            }
            return NotFound();
        }

        private async Task LoadCartForView()
        {
            var result = await _cartService.GetCartAsync(GetJwt());
            if (result.IsSuccess)
            {
                var cartItems = result.Value;
                ViewBag.CartItems = cartItems;
                ViewBag.TotalPrice = cartItems.Sum(c => c.Quantity * c.Price);
            }
        }
    }
}