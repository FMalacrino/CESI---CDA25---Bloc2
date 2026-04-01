using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Security.Data.Models;

namespace Security.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Resource> Resources { get; set; }

#if TEST

        // Simplification de BdD en mémoire
        // Dans l'absolu il faut utiliser le DbContextOptions et uniquement garder le seed
        public DataContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("MyDatabase");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Resource>().HasData(
                new Resource() { Id = 1 });
        }

#elif DEBUG
        public DataContext(DbContextOptions<DataContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Resource>().HasData(
                new Resource() { Id = 1 });
        }
#else
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }
#endif
    }
}