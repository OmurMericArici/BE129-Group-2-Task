using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data;
using Shoppers.Data.Entities;
using Shoppers.Web.Mvc.Models;
using System.Security.Claims;

namespace Shoppers.Web.Mvc.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ShoppersDbContext _context;

        public OrderController(ShoppersDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = GetUserId();

            var cartHasItems = await _context.CartItems.AnyAsync(c => c.UserId == userId);
            if (!cartHasItems)
            {
                return RedirectToAction("Edit", "Cart");
            }

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            ViewBag.CartItems = cartItems;
            ViewBag.TotalPrice = cartItems.Sum(c => c.Quantity * c.Product.Price);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateViewModel model)
        {
            var userId = GetUserId();
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToAction("Edit", "Cart");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CartItems = cartItems;
                ViewBag.TotalPrice = cartItems.Sum(c => c.Quantity * c.Product.Price);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new OrderEntity
                {
                    UserId = userId,
                    Address = model.Address,
                    OrderCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    CreatedAt = DateTime.Now
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in cartItems)
                {
                    if (item.Product.StockAmount < item.Quantity)
                    {
                        throw new Exception($"'{item.Product.Name}' ürünü için yeterli stok kalmadı.");
                    }

                    var orderItem = new OrderItemEntity
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price,
                        CreatedAt = DateTime.Now
                    };

                    _context.OrderItems.Add(orderItem);

                    item.Product.StockAmount -= item.Quantity;
                    _context.Products.Update(item.Product);
                }

                _context.CartItems.RemoveRange(cartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction("Details", new { id = order.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Sipariş oluşturulurken bir hata oluştu: " + ex.Message);

                ViewBag.CartItems = cartItems;
                ViewBag.TotalPrice = cartItems.Sum(c => c.Quantity * c.Product.Price);
                return View(model);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = GetUserId();
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null) return NotFound();

            return View(order);
        }
    }
}