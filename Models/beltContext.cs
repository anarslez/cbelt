using Microsoft.EntityFrameworkCore;
 
namespace belt.Models
{
    public class beltContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public beltContext(DbContextOptions<beltContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Join> Joins { get; set; }
    } 
}