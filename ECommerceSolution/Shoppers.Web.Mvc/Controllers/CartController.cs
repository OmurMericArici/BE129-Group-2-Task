using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data;
using Shoppers.Data.Entities;
using Shoppers.Web.Mvc.Models;

namespace Shoppers.Web.Mvc.Controllers
{
    public class CartController : Controller
    {
        private readonly ShoppersDbContext _context;

        public CartController(ShoppersDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(int productId, int quantity = 1)
        {
            var userId = 1;

            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            if (product.StockAmount < quantity)
            {
                return RedirectToAction(nameof(Edit));
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem != null)
            {
                if (product.StockAmount >= cartItem.Quantity + quantity)
                {
                    cartItem.Quantity += (byte)quantity;
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
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit));
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = 1;

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .ThenInclude(p => p.Images)
                .Where(c => c.UserId == userId)
                .ToListAsync();

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
        public async Task<IActionResult> UpdateCart(Dictionary<int, int> quantities)
        {
            foreach (var quantity in quantities)
            {
                var cartItem = await _context.CartItems
                    .Include(c => c.Product)
                    .FirstOrDefaultAsync(c => c.Id == quantity.Key);

                if (cartItem != null)
                {
                    if (quantity.Value > 0 && quantity.Value <= cartItem.Product.StockAmount)
                    {
                        cartItem.Quantity = (byte)quantity.Value;
                    }
                }
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit));
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit));
        }
    }
}