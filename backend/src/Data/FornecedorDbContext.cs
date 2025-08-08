using Microsoft.EntityFrameworkCore;
using MyApp.Models;

namespace MyApp.Data
{
    public class FornecedorDbContext : DbContext
    {
        public FornecedorDbContext(DbContextOptions<FornecedorDbContext> options) : base(options)
        {
        }

        public DbSet<Fornecedor> Fornecedores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fornecedor>(entity =>
            {
                entity.ToTable("Fornecedores");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Cnpj)
                      .IsRequired()
                      .HasMaxLength(14)
                      .IsUnicode(false); // para suportar caracteres alfanuméricos
            });
        }
    }
}
