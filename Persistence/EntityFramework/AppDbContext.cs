using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.EntityFramework
{
    public class AppDbContext : DbContext
    {
        public DbSet<Address> Addresses => Set<Address>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var cepConverter = new ValueConverter<Cep, string>(
                v => v.Value,
                v => Cep.From(v)
            );

            modelBuilder.Entity<Address>(e =>
            {
                e.ToTable("Addresses");
                e.HasKey(x => x.Id);

                e.Property(x => x.Cep)
                    .HasConversion(cepConverter)
                    .HasMaxLength(8)
                    .IsRequired();

                e.Property(x => x.Street).HasMaxLength(200).IsRequired();
                e.Property(x => x.Number).HasMaxLength(50).IsRequired();
                e.Property(x => x.Complement).HasMaxLength(200);
                e.Property(x => x.Neighborhood).HasMaxLength(120).IsRequired();
                e.Property(x => x.City).HasMaxLength(120).IsRequired();
                e.Property(x => x.State).HasMaxLength(2).IsRequired();
            });
        }
    }
}
