using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PablitoJere.Entities;

namespace PablitoJere
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Publication> Publications { get; set; }
        public DbSet<PublicationImage> PublicationImages { get; set; }
        public DbSet<Comment> Comments { get; set; }

    }
}
