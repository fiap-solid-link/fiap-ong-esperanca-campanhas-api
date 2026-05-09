using Esperanca.Campanha.Domain.Doacoes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Esperanca.Campanha.Infrastructure.Doacoes.Persistence;

public class ArrecadacaoProcessadaConfiguration : IEntityTypeConfiguration<ArrecadacaoProcessada>
{
    public void Configure(EntityTypeBuilder<ArrecadacaoProcessada> builder)
    {
        builder.ToTable("arrecadacoes_processadas");
        builder.HasKey(a => a.IdDoacao);

        builder.Property(a => a.IdDoacao).ValueGeneratedNever();
        builder.Property(a => a.IdCampanha).IsRequired();
        builder.Property(a => a.Valor).HasColumnType("numeric(18,2)").IsRequired();
        builder.Property(a => a.DataProcessamento).IsRequired();

        builder.HasIndex(a => a.IdCampanha);
    }
}
