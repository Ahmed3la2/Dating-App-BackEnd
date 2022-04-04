using BackEnd.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<AppUser> Users { get; set; }

        
    }
}
