using App.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Shoppers.Web.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public HomeController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _productService.GetAllAsync();
            if (result.IsSuccess)
            {
                var featured = result.Value.Take(6).ToList();
                return View(featured);
            }
            return View(new List<App.Models.DTO.ProductDto>());
        }

        public async Task<IActionResult> Listing(string category, string sort)
        {
            var productResult = await _productService.GetAllAsync();
            var products = productResult.IsSuccess ? productResult.Value : new List<App.Models.DTO.ProductDto>();

            var categoryResult = await _categoryService.GetAllAsync();
            var categories = categoryResult.IsSuccess ? categoryResult.Value : new List<App.Models.DTO.CategoryDto>();

            var query = products.AsQueryable();

            if (!string.IsNullOrEmpty(category) && category != "all")
            {
                query = query.Where(p => p.CategoryName == category);
            }

            switch (sort)
            {
                case "newest": query = query.OrderByDescending(p => p.Id); break;
                case "price_asc": query = query.OrderBy(p => p.Price); break;
                case "price_desc": query = query.OrderByDescending(p => p.Price); break;
                default: query = query.OrderByDescending(p => p.Id); break;
            }

            ViewBag.Categories = categories;
            ViewBag.CurrentCategory = category;

            return View(query.ToList());
        }

        public async Task<IActionResult> ProductDetail(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (result.IsSuccess)
            {
                return View(result.Value);
            }
            return NotFound();
        }

        public IActionResult AboutUs() => View();
        public IActionResult Contact() => View();
    }
}