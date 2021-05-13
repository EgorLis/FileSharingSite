using Microsoft.EntityFrameworkCore;
using FileSharingSite.Models;

namespace FileSharingSite.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<FileModel> File { get; set; }
        public DbSet<CatalogModel> Catalog { get; set; }
        public DbSet<UserModel> User { get; set; }
    }
}

