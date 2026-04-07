using Fiap.OngEsperanca.Campanhas.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Repositories;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational.Repositories;
using FluentValidation;
using System.Reflection;

namespace Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;

public class CampanhasDbContext : DbContext
{
    public CampanhasDbContext(DbContextOptions<CampanhasDbContext> options) : base(options)
    {
    }

    // A representação da nossa tabela no banco de dados
    public DbSet<Campanha> Campanhas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapeamento via Fluent API (protege a entidade de atributos do banco)
        modelBuilder.Entity<Campanha>(entity =>
        {
            entity.ToTable("Campanhas");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Titulo)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(c => c.Descricao)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(c => c.DataInicio)
                .IsRequired();

            entity.Property(c => c.DataFim)
                .IsRequired();

            // Configuração de precisão para valores monetários (obrigatório para decimals)
            entity.Property(c => c.MetaFinanceira)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(c => c.ValorTotalArrecadado)
                .HasPrecision(18, 2)
                .IsRequired();

            // Salvando o Enum como String para facilitar a leitura direto no banco (ex: "Ativa" em vez de 0)
            entity.Property(c => c.Status)
                .HasConversion<string>()
                .IsRequired();
        });
    }
}