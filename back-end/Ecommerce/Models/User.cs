﻿namespace Ecommerce.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        // Navigation property
        public ICollection<Order> Orders { get; set; }
    }
}
