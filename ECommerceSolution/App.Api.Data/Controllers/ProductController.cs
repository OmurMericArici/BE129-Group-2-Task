using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppers.Data.Entities;
using Shoppers.Data.Repositories;
using System.Security.Claims;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IRepository<ProductEntity> _productRepository;

        public ProductController(IRepository<ProductEntity> productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _productRepository.GetAll()
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.Enabled)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
            return Ok(products);
        }

        [HttpGet("seller/{sellerId}")]
        [Authorize(Roles = "Seller")]
        public IActionResult GetBySeller(int sellerId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (currentUserId != sellerId) return Forbid();

            var products = _productRepository.GetAll()
                .Include(p => p.Images)
                .Where(p => p.SellerId == sellerId && p.Enabled)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var product = _productRepository.GetAll()
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public IActionResult Create(ProductEntity product)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            product.SellerId = currentUserId;
            product.CreatedAt = DateTime.Now;
            product.Enabled = true;

            _productRepository.Add(product);
            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }

        [HttpPut]
        [Authorize(Roles = "Seller")]
        public IActionResult Update(ProductEntity product)
        {
            var existing = _productRepository.GetAll().Include(p => p.Images).FirstOrDefault(p => p.Id == product.Id);
            if (existing == null) return NotFound();

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (existing.SellerId != currentUserId) return Forbid();

            existing.Name = product.Name;
            existing.Price = product.Price;
            existing.StockAmount = product.StockAmount;
            existing.Details = product.Details;
            existing.CategoryId = product.CategoryId;

            _productRepository.Update(existing);
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller,Admin")]
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null) return NotFound();

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            if (userRole == "Seller" && product.SellerId != currentUserId)
            {
                return Forbid();
            }

            product.Enabled = false;
            _productRepository.Update(product);
            return NoContent();
        }
    }
}