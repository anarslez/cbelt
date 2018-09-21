using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace belt.Models
{
    public class Login
    {
        [NotMapped]
        [Required]
        [EmailAddress]
        public string Email { get; set;  }
        [NotMapped]
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}