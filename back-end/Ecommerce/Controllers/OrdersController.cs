using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models; // Adjust the namespace according to your project structure
using System.Linq;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly MyDbContext _context;

        public OrdersController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public IActionResult GetAllOrders()
        {
            var orders = _context.Orders.ToList();
            return Ok(orders);
        }

        // GET: api/Orders?id={id}
        [HttpGet("id")]
        public IActionResult GetOrderById([FromQuery] int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID must be greater than 0.");
            }

            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // GET: api/Orders?name={name}
        [HttpGet("name")]
        public IActionResult GetOrderByName([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name cannot be null or empty.");
            }

            var order = _context.Orders.FirstOrDefault(o => o.User.Username == name);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // DELETE: api/Orders?id={id}
        [HttpDelete("id")]
        public IActionResult DeleteOrder([FromQuery] int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID must be greater than 0.");
            }

            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
