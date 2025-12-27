using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Shoppers.Web.AdminMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IRepository<ProductEntity> _productRepository;

        public ProductController(IRepository<ProductEntity> productRepository)
        {
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            var products = _productRepository.GetAll()
                                             .Include(p => p.Category)
                                             .Include(p => p.Seller)
                                             .ToList();
            return View(products);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetById(id);
            if (product != null)
            {
                product.Enabled = false;
                _productRepository.Update(product);
            }
            return RedirectToAction("Index");
        }
    }
}