using App.Models.DTO;
using App.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shoppers.Web.AdminMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        private string GetJwt() => Request.Cookies["ShoppersAdminToken"]!;

        public async Task<IActionResult> Index()
        {
            var result = await _service.GetAllAsync();
            if (result.IsSuccess) return View(result.Value);
            return View(new List<ProductDto>());
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(GetJwt(), id);
            return RedirectToAction("Index");
        }
    }
}