using Microsoft.EntityFrameworkCore;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.DB
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
    }
}