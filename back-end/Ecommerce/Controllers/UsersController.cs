using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models; // Adjust the namespace according to your project structure
using System.Linq;
using Ecommerce.DTO; // Adjust the namespace according to your project structure

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly MyDbContext _context;

        public UsersController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }

        // GET: api/Users/{id}
        [HttpGet("{id:int}")]
        public IActionResult GetUserById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID must be greater than 0.");
            }

            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // GET: api/Users/name/{name}
        [HttpGet("name/{name}")]
        public IActionResult GetUserByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name cannot be null or empty.");
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == name);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST: api/Users
        [HttpPost]
        public IActionResult AddUser([FromBody] UserDTO userDto)
        {
            if (userDto == null)
            {
                return BadRequest("User data is null.");
            }

            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                Password = userDto.Password
                // Set other properties here
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
        }

        // PUT: api/Users/{id}
        [HttpPut("{id:int}")]
        public IActionResult UpdateUser(int id, [FromBody] UserDTO userDto)
        {
            if (id <= 0)
            {
                return BadRequest("ID must be greater than 0.");
            }

            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            // Update user properties
            user.Username = userDto.Username;
            user.Email = userDto.Email;
            user.Password = userDto.Password;
            // Update other properties as needed

            _context.SaveChanges();

            return Ok(user);
        }

        // DELETE: api/Users/{id}
        [HttpDelete("{id:int}")]
        public IActionResult DeleteUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID must be greater than 0.");
            }

            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
