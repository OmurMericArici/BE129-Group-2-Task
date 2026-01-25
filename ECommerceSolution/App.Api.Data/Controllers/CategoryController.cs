using App.Api.Data.Services.Abstract;
using App.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryApiService _service;

        public CategoryController(ICategoryApiService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _service.GetAll();
            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = _service.GetById(id);
            if (!result.IsSuccess) return NotFound();
            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(CategoryCreateDto model)
        {
            _service.Create(model);
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(CategoryUpdateDto model)
        {
            var result = _service.Update(model);
            if (!result.IsSuccess) return NotFound();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var result = _service.Delete(id);
            if (!result.IsSuccess) return NotFound();
            return NoContent();
        }
    }
}