using Mi_punto_de_venta.Data;
using Mi_punto_de_venta.DTOs;
using Mi_punto_de_venta.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;


namespace Mi_punto_de_venta.Controllers
{
  
    [ApiController]
    [Route("api/products")]
    public class ProductsController : Controller
    {

        private readonly PosDbContext _context;

        public ProductsController(PosDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_context.Products.ToList());
        }

        [HttpPost]
        public IActionResult Create(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                Size = dto.Size,
                Color = dto.Color,
                ImageUrl = dto.ImageUrl,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return Ok(product);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CreateProductDto dto)
        {
            var product = _context.Products.Find(id);

            if (product == null)
                return NotFound();

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.Size = dto.Size;
            product.Color = dto.Color;

            _context.SaveChanges();

            return Ok(product);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
                return NotFound();

            product.IsActive = false;

            _context.SaveChanges();

            return Ok("Producto desactivado");
        }

        //Moztrar los productos activos nadamas 
        /*
        [HttpGet]
        public IActionResult GetActive()
        {
            var products = _context.Products
                .Where(p => p.IsActive)
                .ToList();

            return Ok(products);
        }
        */
    }

}

