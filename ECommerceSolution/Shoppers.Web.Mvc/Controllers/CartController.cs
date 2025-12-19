using Microsoft.AspNetCore.Authorization; // Eklendi
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data;
using Shoppers.Data.Entities;
using Shoppers.Web.Mvc.Models;
using System.Security.Claims; // Eklendi

namespace Shoppers.Web.Mvc.Controllers
{
    [Authorize] // Sepet işlemleri için giriş şartı
    public class CartController : Controller
    {
        private readonly ShoppersDbContext _context;

        public CartController(ShoppersDbContext context)
        {
            _context = context;
        }

        // Yardımcı Metot
        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(int productId, int quantity = 1)
        {
            var userId = GetUserId();

            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            if (product.StockAmount < quantity)
            {
                TempData["Error"] = "Yetersiz stok!";
                return RedirectToAction(nameof(Edit)); // veya ProductDetail
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem != null)
            {
                if (product.StockAmount >= cartItem.Quantity + quantity)
                {
                    cartItem.Quantity += (byte)quantity;
                }
                else
                {
                    TempData["Error"] = "Stok sınırına ulaşıldı.";
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
            var userId = GetUserId();

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
            var userId = GetUserId(); // Güvenlik kontrolü için kullanıcıya ait mi diye bakılabilir

            foreach (var quantity in quantities)
            {
                var cartItem = await _context.CartItems
                    .Include(c => c.Product)
                    .FirstOrDefaultAsync(c => c.Id == quantity.Key && c.UserId == userId);

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
            var userId = GetUserId();
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit));
        }
    }
}