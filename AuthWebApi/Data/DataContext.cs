using AuthWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthWebApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext( DbContextOptions<DataContext> options) : base(options) 
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
