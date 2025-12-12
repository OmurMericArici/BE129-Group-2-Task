using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data;
using Shoppers.Data.Entities;
using Shoppers.Web.Mvc.Models;

namespace Shoppers.Web.Mvc.Controllers
{
    public class ProductController : Controller
    {
        private readonly ShoppersDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductController(ShoppersDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // --- CREATE ---
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
                return View(model);
            }

            var userId = 1; // Simulated Logged In User (Seller)

            // 1. Create Product
            var product = new ProductEntity
            {
                Name = model.Name,
                Price = model.Price,
                StockAmount = (byte)model.StockAmount,
                CategoryId = model.CategoryId,
                Details = model.Details,
                SellerId = userId,
                CreatedAt = DateTime.Now,
                Enabled = true
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // 2. Handle Image Upload
            if (model.ImageFile != null)
            {
                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                var path = Path.Combine(_environment.WebRootPath, "images", newFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                var image = new ProductImageEntity
                {
                    ProductId = product.Id,
                    Url = "/images/" + newFileName,
                    CreatedAt = DateTime.Now
                };
                _context.ProductImages.Add(image);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyProducts", "Profile");
        }

        // --- EDIT ---
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Include(p => p.Images).FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            var model = new ProductEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                StockAmount = product.StockAmount,
                Details = product.Details,
                CategoryId = product.CategoryId,
                CurrentImageUrl = product.Images.FirstOrDefault()?.Url
            };

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
                return View(model);
            }

            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == model.Id);
            if (product == null) return NotFound();

            product.Name = model.Name;
            product.Price = model.Price;
            product.StockAmount = (byte)model.StockAmount;
            product.Details = model.Details;
            product.CategoryId = model.CategoryId;

            if (model.ImageFile != null)
            {
                // Simple logic: Upload new, replace old reference (In real app, might want to delete old file)
                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                var path = Path.Combine(_environment.WebRootPath, "images", newFileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                // If exists, update; else add
                var img = product.Images.FirstOrDefault();
                if (img != null)
                {
                    img.Url = "/images/" + newFileName;
                    _context.Update(img);
                }
                else
                {
                    _context.ProductImages.Add(new ProductImageEntity { ProductId = product.Id, Url = "/images/" + newFileName, CreatedAt = DateTime.Now });
                }
            }

            _context.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyProducts", "Profile");
        }

        // --- DELETE ---
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Include(p => p.Images).FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                // Soft delete based on Task 11 Scenario 22
                product.Enabled = false;
                _context.Update(product);
                _context.SaveChanges();
            }
            return RedirectToAction("MyProducts", "Profile");
        }

        // --- COMMENT ---
        [HttpGet]
        public IActionResult Comment(int id)
        {
            var product = _context.Products.Include(p => p.Images).FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            ViewBag.ProductName = product.Name;
            ViewBag.ProductImage = product.Images.FirstOrDefault()?.Url;
            ViewBag.Price = product.Price;

            return View(new ProductCommentViewModel { ProductId = id });
        }

        [HttpPost]
        public IActionResult Comment(ProductCommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Re-fetch info for display
                var product = _context.Products.Include(p => p.Images).FirstOrDefault(p => p.Id == model.ProductId);
                ViewBag.ProductName = product?.Name;
                ViewBag.ProductImage = product?.Images.FirstOrDefault()?.Url;
                return View(model);
            }

            var comment = new ProductCommentEntity
            {
                ProductId = model.ProductId,
                UserId = 1, // Hardcoded user
                Text = model.Text,
                StarCount = model.StarCount,
                IsConfirmed = false,
                CreatedAt = DateTime.Now
            };

            _context.ProductComments.Add(comment);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Comment submitted for approval!";
            return RedirectToAction("Listing", "Home");
        }
    }
}