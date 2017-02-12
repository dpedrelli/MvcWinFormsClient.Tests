using Microsoft.EntityFrameworkCore;
using Models;

namespace MvcApi1.Data
{
    public class MvcApi1DbContext : DbContext
    {
        public MvcApi1DbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products", "dbo");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasDefaultValue("")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Price)
                    .IsRequired()
                    .HasDefaultValue(0.0);
            });
        }

        public virtual DbSet<Product> Products { get; set; }
    }
}
