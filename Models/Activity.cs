using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;
namespace belt.Models
{
    public class ValidDateAttribute : ValidationAttribute 
    {
        public override bool IsValid(object value)
        {
            DateTime d = Convert.ToDateTime(value);
            return d >= DateTime.Now;
        }
    }
    public class Activity
    {
        [Key]
        public int ActivityId { get; set; }
        [Required]
        [MinLength(2)]
        public string Title { get; set; }
        [Required]
        [ValidDate (ErrorMessage = "Your activity already happened what are you planning!?")]
        public DateTime Date { get; set; }
        public DateTime Endtime { get; set; }
        [Required]
        [Range(0,1000000000000)]
        public int Duration { get; set; }
        [Required]
        public string Units {get; set;}
        [Required]
        [MinLength(10)]
        public string Description { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public List<Join> Joins { get; set; }
    }
}