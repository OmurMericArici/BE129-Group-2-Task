using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;
using Shoppers.Web.AdminMvc.Models;

namespace Shoppers.Web.AdminMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IRepository<CategoryEntity> _categoryRepository;

        public CategoryController(IRepository<CategoryEntity> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public IActionResult List()
        {
            var categories = _categoryRepository.GetAll().ToList();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CategoryCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = new CategoryEntity
            {
                Name = model.Name,
                Color = model.Color,
                IconCssClass = model.IconCssClass,
                CreatedAt = DateTime.Now
            };

            _categoryRepository.Add(entity);

            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var entity = _categoryRepository.GetById(id);
            if (entity == null) return NotFound();

            var model = new CategoryEditViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Color = entity.Color,
                IconCssClass = entity.IconCssClass
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(CategoryEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = _categoryRepository.GetById(model.Id);
            if (entity == null) return NotFound();

            entity.Name = model.Name;
            entity.Color = model.Color;
            entity.IconCssClass = model.IconCssClass;

            _categoryRepository.Update(entity);

            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var entity = _categoryRepository.GetById(id);
            if (entity == null) return NotFound();

            return View(entity);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            _categoryRepository.Delete(id);
            return RedirectToAction("List");
        }
    }
}