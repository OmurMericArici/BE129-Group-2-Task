using Microsoft.AspNetCore.Mvc;
using Shoppers.Data;
using Shoppers.Data.Entities;
using Shoppers.Web.AdminMvc.Models;

namespace Shoppers.Web.AdminMvc.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ShoppersDbContext _context;

        public CategoryController(ShoppersDbContext context)
        {
            _context = context;
        }

        public IActionResult List()
        {
            var categories = _context.Categories.ToList();
            return View(categories); // You might want to create a listing view later
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

            _context.Categories.Add(entity);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home"); // Redirect to Dashboard or List
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var entity = _context.Categories.Find(id);
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

            var entity = _context.Categories.Find(model.Id);
            if (entity == null) return NotFound();

            entity.Name = model.Name;
            entity.Color = model.Color;
            entity.IconCssClass = model.IconCssClass;

            _context.Categories.Update(entity);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var entity = _context.Categories.Find(id);
            if (entity == null) return NotFound();

            return View(entity); // We can pass entity directly to delete view for display
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var entity = _context.Categories.Find(id);
            if (entity != null)
            {
                _context.Categories.Remove(entity);
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}