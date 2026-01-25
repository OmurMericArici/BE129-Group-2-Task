using App.Models.DTO;
using App.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppers.Web.AdminMvc.Models;

namespace Shoppers.Web.AdminMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        private string GetJwt() => Request.Cookies["ShoppersAdminToken"]!;

        public async Task<IActionResult> List()
        {
            var result = await _service.GetAllAsync();
            if (result.IsSuccess) return View(result.Value);
            return View(new List<CategoryDto>());
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = new CategoryCreateDto
            {
                Name = model.Name,
                Color = model.Color,
                IconCssClass = model.IconCssClass
            };

            var result = await _service.CreateAsync(GetJwt(), dto);
            if (result.IsSuccess) return RedirectToAction("List");

            ModelState.AddModelError("", "Error creating category.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result.IsSuccess)
            {
                var dto = result.Value;
                var model = new CategoryEditViewModel
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Color = dto.Color,
                    IconCssClass = dto.IconCssClass
                };
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = new CategoryUpdateDto
            {
                Id = model.Id,
                Name = model.Name,
                Color = model.Color,
                IconCssClass = model.IconCssClass
            };

            var result = await _service.UpdateAsync(GetJwt(), dto);
            if (result.IsSuccess) return RedirectToAction("List");

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result.IsSuccess) return View(result.Value);
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(GetJwt(), id);
            return RedirectToAction("List");
        }
    }
}