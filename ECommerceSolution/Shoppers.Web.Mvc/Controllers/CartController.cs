using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;
using Shoppers.Web.Mvc.Models;
using System.Security.Claims;

namespace Shoppers.Web.Mvc.Controllers
{
    [Authorize(Roles = "Buyer,Seller")]
    public class CartController : Controller
    {
        private readonly IRepository<CartItemEntity> _cartRepository;
        private readonly IRepository<ProductEntity> _productRepository;

        public CartController(
            IRepository<CartItemEntity> cartRepository,
            IRepository<ProductEntity> productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        [HttpPost]
        public IActionResult AddProduct(int productId, int quantity = 1)
        {
            var userId = GetUserId();

            var product = _productRepository.GetById(productId);
            if (product == null) return NotFound();

            if (product.StockAmount < quantity)
            {
                TempData["Error"] = "Insufficient stock!";
                return RedirectToAction(nameof(Edit));
            }

            var cartItem = _cartRepository.Get(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem != null)
            {
                if (product.StockAmount >= cartItem.Quantity + quantity)
                {
                    cartItem.Quantity += (byte)quantity;
                    _cartRepository.Update(cartItem);
                }
                else
                {
                    TempData["Error"] = "Stock limit reached.";
                }
            }
            else
            {
                cartItem = new CartItemEntity
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = (byte)quantity,
                    CreatedAt = DateTime.Now
                };
                _cartRepository.Add(cartItem);
            }

            return RedirectToAction(nameof(Edit));
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var userId = GetUserId();

            var cartItems = _cartRepository.GetAll()
                .Include(c => c.Product)
                .ThenInclude(p => p.Images)
                .Where(c => c.UserId == userId)
                .ToList();

            var viewModel = new CartViewModel
            {
                Items = cartItems.Select(c => new CartItemViewModel
                {
                    CartItemId = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    Price = c.Product.Price,
                    Quantity = c.Quantity,
                    StockAvailable = c.Product.StockAmount,
                    ImageUrl = c.Product.Images.FirstOrDefault()?.Url
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult UpdateCart(Dictionary<int, int> quantities)
        {
            var userId = GetUserId();

            foreach (var quantity in quantities)
            {
                var cartItem = _cartRepository.GetAll()
                    .Include(c => c.Product)
                    .FirstOrDefault(c => c.Id == quantity.Key && c.UserId == userId);

                if (cartItem != null)
                {
                    if (quantity.Value > 0 && quantity.Value <= cartItem.Product.StockAmount)
                    {
                        cartItem.Quantity = (byte)quantity.Value;
                        _cartRepository.Update(cartItem);
                    }
                }
            }

            return RedirectToAction(nameof(Edit));
        }

        [HttpPost]
        public IActionResult Remove(int cartItemId)
        {
            var userId = GetUserId();
            var cartItem = _cartRepository.Get(c => c.Id == cartItemId && c.UserId == userId);

            if (cartItem != null)
            {
                _cartRepository.Delete(cartItem);
            }

            return RedirectToAction(nameof(Edit));
        }
    }
}