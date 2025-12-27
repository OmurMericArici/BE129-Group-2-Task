using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;

namespace Shoppers.Web.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<ProductEntity> _productRepository;
        private readonly IRepository<CategoryEntity> _categoryRepository;

        public HomeController(IRepository<ProductEntity> productRepository, IRepository<CategoryEntity> categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            var products = _productRepository.GetAll()
                                   .Include(p => p.Images)
                                   .Where(p => p.Enabled)
                                   .OrderByDescending(p => p.CreatedAt)
                                   .Take(6)
                                   .ToList();
            return View(products);
        }

        public IActionResult Listing(string category, string sort)
        {
            var query = _productRepository.GetAll()
                                .Include(p => p.Images)
                                .Include(p => p.Category)
                                .Where(p => p.Enabled);

            if (!string.IsNullOrEmpty(category) && category != "all")
            {
                query = query.Where(p => p.Category.Name == category);
            }

            switch (sort)
            {
                case "newest":
                    query = query.OrderByDescending(p => p.CreatedAt);
                    break;
                case "price_asc":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                default:
                    query = query.OrderByDescending(p => p.CreatedAt);
                    break;
            }

            ViewBag.Categories = _categoryRepository.GetAll().OrderBy(c => c.Name).ToList();
            ViewBag.CurrentCategory = category;

            return View(query.ToList());
        }

        public IActionResult ProductDetail(int id)
        {
            var product = _productRepository.GetAll()
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Comments)
                .FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}