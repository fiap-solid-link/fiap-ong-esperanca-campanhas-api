using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CampanhaAgg = Esperanca.Campanha.Domain.Campanhas.Campanha;

namespace Esperanca.Campanha.Infrastructure.Campanhas;

public class CampanhaConfiguration : IEntityTypeConfiguration<CampanhaAgg>
{
    public void Configure(EntityTypeBuilder<CampanhaAgg> builder)
    {
        builder.ToTable("campanhas");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Titulo).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Descricao).HasMaxLength(2000).IsRequired();
        builder.Property(c => c.DataInicio).IsRequired();
        builder.Property(c => c.DataFim).IsRequired();
        builder.Property(c => c.MetaFinanceira).HasColumnType("numeric(18,2)").IsRequired();
        builder.Property(c => c.ModoEncerramento).IsRequired();
        builder.Property(c => c.Status).IsRequired();
        builder.Property(c => c.ValorArrecadado).HasColumnType("numeric(18,2)").IsRequired();
        builder.Property(c => c.IdGestor).IsRequired();

        builder.HasIndex(c => c.IdGestor);
        builder.HasIndex(c => c.Status);
    }
}
