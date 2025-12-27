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
    public class OrderController : Controller
    {
        private readonly IRepository<OrderEntity> _orderRepository;
        private readonly IRepository<OrderItemEntity> _orderItemRepository;
        private readonly IRepository<CartItemEntity> _cartRepository;
        private readonly IRepository<ProductEntity> _productRepository;

        public OrderController(
            IRepository<OrderEntity> orderRepository,
            IRepository<OrderItemEntity> orderItemRepository,
            IRepository<CartItemEntity> cartRepository,
            IRepository<ProductEntity> productRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var userId = GetUserId();

            var cartHasItems = _cartRepository.Any(c => c.UserId == userId);
            if (!cartHasItems)
            {
                return RedirectToAction("Edit", "Cart");
            }

            var cartItems = _cartRepository.GetAll()
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();

            ViewBag.CartItems = cartItems;
            ViewBag.TotalPrice = cartItems.Sum(c => c.Quantity * c.Product.Price);

            return View();
        }

        [HttpPost]
        public IActionResult Create(OrderCreateViewModel model)
        {
            var userId = GetUserId();
            var cartItems = _cartRepository.GetAll()
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();

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

            try
            {
                var order = new OrderEntity
                {
                    UserId = userId,
                    Address = model.Address,
                    OrderCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    CreatedAt = DateTime.Now
                };

                _orderRepository.Add(order);

                foreach (var item in cartItems)
                {
                    if (item.Product.StockAmount < item.Quantity)
                    {
                        throw new Exception($"Insufficient stock for '{item.Product.Name}'.");
                    }

                    var orderItem = new OrderItemEntity
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price,
                        CreatedAt = DateTime.Now
                    };

                    _orderItemRepository.Add(orderItem);

                    item.Product.StockAmount -= item.Quantity;
                    _productRepository.Update(item.Product);
                }

                foreach (var item in cartItems)
                {
                    _cartRepository.Delete(item);
                }

                return RedirectToAction("Details", new { id = order.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the order: " + ex.Message);

                ViewBag.CartItems = cartItems;
                ViewBag.TotalPrice = cartItems.Sum(c => c.Quantity * c.Product.Price);
                return View(model);
            }
        }

        public IActionResult Details(int id)
        {
            var userId = GetUserId();
            var order = _orderRepository.GetAll()
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.Id == id && o.UserId == userId);

            if (order == null) return NotFound();

            return View(order);
        }
    }
}