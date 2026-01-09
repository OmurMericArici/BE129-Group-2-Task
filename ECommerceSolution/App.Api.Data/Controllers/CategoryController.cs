using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IRepository<CategoryEntity> _categoryRepository;

        public CategoryController(IRepository<CategoryEntity> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _categoryRepository.GetAll().OrderBy(c => c.Name).ToList();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var category = _categoryRepository.GetById(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(CategoryEntity category)
        {
            category.CreatedAt = DateTime.Now;
            _categoryRepository.Add(category);
            return CreatedAtAction(nameof(Get), new { id = category.Id }, category);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(CategoryEntity category)
        {
            var existing = _categoryRepository.GetById(category.Id);
            if (existing == null) return NotFound();

            existing.Name = category.Name;
            existing.Color = category.Color;
            existing.IconCssClass = category.IconCssClass;

            _categoryRepository.Update(existing);
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var existing = _categoryRepository.GetById(id);
            if (existing == null) return NotFound();

            _categoryRepository.Delete(existing);
            return NoContent();
        }
    }
}