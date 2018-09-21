using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
namespace belt.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [MinLength(2)]
        public string FirstName { get; set; }
        [Required]
        [MinLength(2)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set;  }
        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^a-zA-Z0-9])(?!.*\s).{8,20}$", ErrorMessage = "Please include one number, one letter, and one special character")]
        [MinLength(8)]
        public string Password { get; set; }
        [NotMapped]
        [Compare("Password", ErrorMessage = "Passwords must match")]
        public string ConPassword { get; set;}
        public List<Activity> Activities { get; set; }
        public List<Join> Joins { get; set; }
    }
}