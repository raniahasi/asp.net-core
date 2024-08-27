using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models; // Adjust the namespace according to your project structure
using System.IO;
using System.Linq;
using Ecommerce.DTO;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public CategoriesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public IActionResult GetAllCategories()
        {
            var categories = _context.Categories.ToList();
            return Ok(categories);
        }

        // GET: api/Categories/{id}
        [HttpGet("{id:int:min(5)}")] // Constraint: id must be >= 5
        public IActionResult GetCategoryById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID must be greater than 0.");
            }

            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // GET: api/Categories/name/{name}
        [HttpGet("name/{name}")]
        public IActionResult GetCategoryByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name cannot be null or empty.");
            }

            var category = _context.Categories.FirstOrDefault(c => c.CategoryName == name);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // DELETE: api/Categories/{id}
        [HttpDelete("{id:int:min(5)}")] // Constraint: id must be >= 5
        public IActionResult DeleteCategory(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID must be greater than 0.");
            }

            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return NoContent();
        }

        // POST: api/Categories
        [HttpPost]
        public IActionResult AddCategory([FromForm] CategoryDto categoryDto)
        {
            string fileName = null;

            if (categoryDto.CategoryImage != null)
            {
                // Define the uploads directory within wwwroot
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images");

                // Check if the directory exists, if not, create it
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                // Generate a unique filename and save the file
                fileName = Guid.NewGuid().ToString() + Path.GetExtension(categoryDto.CategoryImage.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    categoryDto.CategoryImage.CopyTo(stream);
                }
            }

            var category = new Category
            {
                CategoryName = categoryDto.CategoryName,
                CategoryImage = fileName != null ? "/uploads/images/" + fileName : null
            };

            _context.Categories.Add(category);
            _context.SaveChanges();

            return Ok(category);
        }

        // PUT: api/Categories/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, [FromForm] CategoryDto categoryDto)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            if (categoryDto.CategoryImage != null)
            {
                // Define the uploads directory within wwwroot
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images");

                // Check if the directory exists, if not, create it
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                // Generate a unique filename and save the file
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(categoryDto.CategoryImage.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    categoryDto.CategoryImage.CopyTo(stream);
                }

                // Optionally delete the old image file if it exists
                if (!string.IsNullOrEmpty(category.CategoryImage))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", category.CategoryImage.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                category.CategoryImage = "/uploads/images/" + fileName;
            }

            category.CategoryName = categoryDto.CategoryName;
            _context.SaveChanges();

            return Ok(category);
        }
    }
}
