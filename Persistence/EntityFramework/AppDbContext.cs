using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.EntityFramework
{
    public class AppDbContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<Individual> Individuals { get; set; } = null!;
        public DbSet<Company> Companies { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var cepConverter = new ValueConverter<Cep, string>(
                v => v.Value,
                v => Cep.From(v)
            );

            var cpfConverter = new ValueConverter<Cpf, string>(
                v => v.Value,
                v => Cpf.From(v)
            );

            var cnpjConverter = new ValueConverter<Cnpj, string>(
                v => v.Value,
                v => Cnpj.From(v)
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
                e.Property(x => x.Neighborhood).HasMaxLength(120).IsRequired();
                e.Property(x => x.City).HasMaxLength(120).IsRequired();
                e.Property(x => x.State).HasMaxLength(2).IsRequired();

                //1 endereço base por CEP
                e.HasIndex(x => x.Cep).IsUnique();
            });

            modelBuilder.Entity<Individual>(e =>
            {
                e.ToTable("Individuals");
                e.HasKey(x => x.Id);

                e.Property(x => x.Name).HasMaxLength(200).IsRequired();

                e.Property(x => x.Cpf)
                    .HasConversion(cpfConverter)
                    .HasMaxLength(11)
                    .IsRequired();

                e.HasIndex(x => x.Cpf).IsUnique();

                e.Property(x => x.AddressId).IsRequired();

                //número/complemento moram na Person (base)
                e.Property(x => x.AddressNumber).HasMaxLength(50).IsRequired();
                e.Property(x => x.AddressComplement).HasMaxLength(200);

                //FK com Address (sem cascata pra não apagar endereço base)
                e.HasOne(x => x.Address)
                 .WithMany()
                 .HasForeignKey(x => x.AddressId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Company>(e =>
            {
                e.ToTable("Companies");
                e.HasKey(x => x.Id);

                e.Property(x => x.LegalName).HasMaxLength(200).IsRequired();

                e.Property(x => x.Cnpj)
                    .HasConversion(cnpjConverter)
                    .HasMaxLength(14)
                    .IsRequired();

                e.HasIndex(x => x.Cnpj).IsUnique();

                e.Property(x => x.AddressId).IsRequired();

                //número/complemento moram na Person (base)
                e.Property(x => x.AddressNumber).HasMaxLength(50).IsRequired();
                e.Property(x => x.AddressComplement).HasMaxLength(200);

                e.HasOne(x => x.Address)
                 .WithMany()
                 .HasForeignKey(x => x.AddressId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
