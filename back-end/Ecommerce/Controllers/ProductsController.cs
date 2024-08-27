using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models; // Adjust the namespace according to your project structure
using System.IO;
using System.Linq;
using Ecommerce.DTO;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly MyDbContext _context;

        public ProductsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var products = _context.Products.ToList();
            return Ok(products);
        }

        // GET: api/Products/{id}
        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID must be greater than 0.");
            }

            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // GET: api/Products/byname/{name}
        [HttpGet("byname/{name}")]
        public IActionResult GetProductByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name cannot be null or empty.");
            }

            var product = _context.Products.FirstOrDefault(p => p.ProductName == name);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // GET: api/Products/orderedbyprice
        [HttpGet("orderedbyprice")]
        public IActionResult GetProductsOrderedByPrice()
        {
            var products = _context.Products.OrderByDescending(p => p.Price).ToList();
            return Ok(products);
        }

        // DELETE: api/Products?id={id}
        [HttpDelete("id")]
        public IActionResult DeleteProduct([FromQuery] int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID must be greater than 0.");
            }

            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return NoContent();
        }

        // POST: api/Products
        [HttpPost]
        public IActionResult AddProduct([FromForm] ProductDTO productDto)
        {
            string fileName = null;

            if (productDto.ProductImage != null)
            {
                // Define the uploads directory within wwwroot
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images");

                // Check if the directory exists, if not, create it
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                // Generate a unique filename and save the file
                fileName = Guid.NewGuid().ToString() + Path.GetExtension(productDto.ProductImage.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    productDto.ProductImage.CopyTo(stream);
                }
            }

            var product = new Product
            {
                ProductName = productDto.ProductName,
                Description = productDto.Description,
                Price = productDto.Price,
                CategoryId = productDto.CategoryId,
                ProductImage = fileName != null ? "/uploads/images/" + fileName : null
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return Ok(product);
        }

        // PUT: api/Products/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromForm] ProductDTO productDto)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            if (productDto.ProductImage != null)
            {
                // Define the uploads directory within wwwroot
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images");

                // Check if the directory exists, if not, create it
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                // Generate a unique filename and save the file
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(productDto.ProductImage.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    productDto.ProductImage.CopyTo(stream);
                }

                // Optionally delete the old image file if it exists
                if (!string.IsNullOrEmpty(product.ProductImage))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ProductImage.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                product.ProductImage = "/uploads/images/" + fileName;
            }

            product.ProductName = productDto.ProductName;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.CategoryId = productDto.CategoryId;

            _context.SaveChanges();

            return Ok(product);
        }
    }
}
