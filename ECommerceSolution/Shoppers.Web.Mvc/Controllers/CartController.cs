using App.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppers.Web.Mvc.Models;

namespace Shoppers.Web.Mvc.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string GetJwt() => Request.Cookies["ShoppersToken"]!;

        [HttpPost]
        public async Task<IActionResult> AddProduct(int productId, int quantity = 1)
        {
            var result = await _cartService.AddToCartAsync(GetJwt(), productId, quantity);
            if (!result.IsSuccess)
            {
                TempData["Error"] = "Could not add item to cart.";
            }
            return RedirectToAction(nameof(Edit));
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var result = await _cartService.GetCartAsync(GetJwt());
            var cartItems = result.IsSuccess ? result.Value : new List<App.Models.DTO.CartItemDto>();

            var viewModel = new CartViewModel
            {
                Items = cartItems.Select(c => new CartItemViewModel
                {
                    CartItemId = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.ProductName,
                    Price = c.Price,
                    Quantity = c.Quantity,
                    StockAvailable = c.StockAvailable,
                    ImageUrl = c.ImageUrl
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(Dictionary<int, int> quantities)
        {
            await _cartService.UpdateCartAsync(GetJwt(), quantities);
            return RedirectToAction(nameof(Edit));
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            await _cartService.RemoveFromCartAsync(GetJwt(), cartItemId);
            return RedirectToAction(nameof(Edit));
        }
    }
}