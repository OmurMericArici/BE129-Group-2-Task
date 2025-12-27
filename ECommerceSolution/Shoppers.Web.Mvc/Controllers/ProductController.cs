using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;
using Shoppers.Web.Mvc.Models;
using System.Security.Claims;

namespace Shoppers.Web.Mvc.Controllers
{
    public class ProductController : Controller
    {
        private readonly IRepository<ProductEntity> _productRepository;
        private readonly IRepository<CategoryEntity> _categoryRepository;
        private readonly IRepository<ProductImageEntity> _productImageRepository;
        private readonly IRepository<ProductCommentEntity> _productCommentRepository;
        private readonly IWebHostEnvironment _environment;

        public ProductController(
            IRepository<ProductEntity> productRepository,
            IRepository<CategoryEntity> categoryRepository,
            IRepository<ProductImageEntity> productImageRepository,
            IRepository<ProductCommentEntity> productCommentRepository,
            IWebHostEnvironment environment)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productImageRepository = productImageRepository;
            _productCommentRepository = productCommentRepository;
            _environment = environment;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        [Authorize(Roles = "Seller")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_categoryRepository.GetAll(), "Id", "Name");
            return View();
        }

        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_categoryRepository.GetAll(), "Id", "Name");
                return View(model);
            }

            var userId = GetUserId();

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

            _productRepository.Add(product);

            if (model.ImageFile != null)
            {
                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                var imagePath = Path.Combine(_environment.WebRootPath, "images");

                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                }

                var path = Path.Combine(imagePath, newFileName);

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
                _productImageRepository.Add(image);
            }

            return RedirectToAction("MyProducts", "Profile");
        }

        [Authorize(Roles = "Seller")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _productRepository.GetAll()
                                            .Include(p => p.Images)
                                            .FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound();

            if (product.SellerId != GetUserId())
            {
                return Forbid();
            }

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

            ViewBag.Categories = new SelectList(_categoryRepository.GetAll(), "Id", "Name", product.CategoryId);
            return View(model);
        }

        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_categoryRepository.GetAll(), "Id", "Name", model.CategoryId);
                return View(model);
            }

            var product = _productRepository.GetAll()
                                            .Include(p => p.Images)
                                            .FirstOrDefault(p => p.Id == model.Id);

            if (product == null) return NotFound();

            if (product.SellerId != GetUserId())
            {
                return Forbid();
            }

            product.Name = model.Name;
            product.Price = model.Price;
            product.StockAmount = (byte)model.StockAmount;
            product.Details = model.Details;
            product.CategoryId = model.CategoryId;

            if (model.ImageFile != null)
            {
                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                var imagePath = Path.Combine(_environment.WebRootPath, "images");

                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                }

                var path = Path.Combine(imagePath, newFileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                var img = product.Images.FirstOrDefault();
                if (img != null)
                {
                    img.Url = "/images/" + newFileName;
                    _productImageRepository.Update(img);
                }
                else
                {
                    _productImageRepository.Add(new ProductImageEntity
                    {
                        ProductId = product.Id,
                        Url = "/images/" + newFileName,
                        CreatedAt = DateTime.Now
                    });
                }
            }

            _productRepository.Update(product);

            return RedirectToAction("MyProducts", "Profile");
        }

        [Authorize(Roles = "Seller")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetAll()
                                            .Include(p => p.Images)
                                            .FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound();

            if (product.SellerId != GetUserId())
            {
                return Forbid();
            }

            return View(product);
        }

        [Authorize(Roles = "Seller")]
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _productRepository.GetById(id);

            if (product != null)
            {
                if (product.SellerId != GetUserId())
                {
                    return Forbid();
                }

                product.Enabled = false;
                _productRepository.Update(product);
            }
            return RedirectToAction("MyProducts", "Profile");
        }

        [Authorize(Roles = "Buyer,Seller")]
        [HttpGet]
        public IActionResult Comment(int id)
        {
            var product = _productRepository.GetAll()
                                            .Include(p => p.Images)
                                            .FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound();

            ViewBag.ProductName = product.Name;
            ViewBag.ProductImage = product.Images.FirstOrDefault()?.Url;
            ViewBag.Price = product.Price;

            return View(new ProductCommentViewModel { ProductId = id });
        }

        [Authorize(Roles = "Buyer,Seller")]
        [HttpPost]
        public IActionResult Comment(ProductCommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var product = _productRepository.GetAll()
                                                .Include(p => p.Images)
                                                .FirstOrDefault(p => p.Id == model.ProductId);
                ViewBag.ProductName = product?.Name;
                ViewBag.ProductImage = product?.Images.FirstOrDefault()?.Url;
                return View(model);
            }

            var comment = new ProductCommentEntity
            {
                ProductId = model.ProductId,
                UserId = GetUserId(),
                Text = model.Text,
                StarCount = model.StarCount,
                IsConfirmed = false,
                CreatedAt = DateTime.Now
            };

            _productCommentRepository.Add(comment);

            TempData["SuccessMessage"] = "Comment submitted for approval!";
            return RedirectToAction("Listing", "Home");
        }
    }
}