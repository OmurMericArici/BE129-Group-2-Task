using App.Models.DTO;
using App.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shoppers.Web.Mvc.Models;

namespace Shoppers.Web.Mvc.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IFileService _fileService;

        public ProductController(IProductService productService, ICategoryService categoryService, IFileService fileService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _fileService = fileService;
        }

        private string GetJwt() => Request.Cookies["ShoppersToken"]!;

        private async Task PrepareCategoriesViewBag()
        {
            var result = await _categoryService.GetAllAsync();
            if (result.IsSuccess)
            {
                ViewBag.Categories = new SelectList(result.Value, "Id", "Name");
            }
        }

        [Authorize(Roles = "Seller")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareCategoriesViewBag();
            return View();
        }

        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PrepareCategoriesViewBag();
                return View(model);
            }

            string? uploadedUrl = null;
            if (model.ImageFile != null)
            {
                var fileResult = await _fileService.UploadFileAsync(model.ImageFile);
                if (fileResult.IsSuccess) uploadedUrl = fileResult.Value;
            }

            var dto = new ProductCreateDto
            {
                Name = model.Name,
                Price = model.Price,
                StockAmount = model.StockAmount,
                CategoryId = model.CategoryId,
                Details = model.Details,
                ImageUrl = uploadedUrl
            };

            var result = await _productService.CreateAsync(GetJwt(), dto);

            if (result.IsSuccess)
            {
                return RedirectToAction("MyProducts", "Profile");
            }

            ModelState.AddModelError("", "Error creating product.");
            await PrepareCategoriesViewBag();
            return View(model);
        }

        [Authorize(Roles = "Seller")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (!result.IsSuccess) return NotFound();

            var product = result.Value;
            var model = new ProductEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                StockAmount = product.StockAmount,
                Details = product.Details,
                CategoryId = product.CategoryId,
                CurrentImageUrl = product.ImageUrls.FirstOrDefault()
            };

            await PrepareCategoriesViewBag();
            return View(model);
        }

        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PrepareCategoriesViewBag();
                return View(model);
            }

            string? uploadedUrl = null;
            if (model.ImageFile != null)
            {
                var fileResult = await _fileService.UploadFileAsync(model.ImageFile);
                if (fileResult.IsSuccess) uploadedUrl = fileResult.Value;
            }

            var dto = new ProductUpdateDto
            {
                Id = model.Id,
                Name = model.Name,
                Price = model.Price,
                StockAmount = model.StockAmount,
                CategoryId = model.CategoryId,
                Details = model.Details,
                ImageUrl = uploadedUrl
            };

            var result = await _productService.UpdateAsync(GetJwt(), dto);

            if (result.IsSuccess)
            {
                return RedirectToAction("MyProducts", "Profile");
            }

            await PrepareCategoriesViewBag();
            return View(model);
        }

        [Authorize(Roles = "Seller")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (result.IsSuccess) return View(result.Value);
            return NotFound();
        }

        [Authorize(Roles = "Seller")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _productService.DeleteAsync(GetJwt(), id);
            if (result.IsSuccess) return RedirectToAction("MyProducts", "Profile");
            return View();
        }

        [Authorize(Roles = "Buyer,Seller")]
        [HttpGet]
        public async Task<IActionResult> Comment(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (result.IsSuccess)
            {
                var product = result.Value;
                ViewBag.ProductName = product.Name;
                ViewBag.ProductImage = product.ImageUrls.FirstOrDefault();
                ViewBag.Price = product.Price;

                return View(new ProductCommentViewModel { ProductId = id });
            }
            return NotFound();
        }

        [Authorize(Roles = "Buyer,Seller")]
        [HttpPost]
        public IActionResult Comment(ProductCommentViewModel model)
        {
            return RedirectToAction("Listing", "Home");
        }
    }
}