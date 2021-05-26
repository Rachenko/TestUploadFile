using Microsoft.EntityFrameworkCore;

namespace TestUpload.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        public DbSet<TblTest> tblTest { get; set; }
    }
}