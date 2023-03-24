using API.Models.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Data.Configurarion
{
    public class PaisConfiguration : IEntityTypeConfiguration<Pais>
    {
        public void Configure(EntityTypeBuilder<Pais> builder)
        {
            builder.ToTable("Paises");
            builder.Property(p => p.Clave).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Description).IsRequired().HasMaxLength(200);
        }
    }
    public class EstadoConfiguration : IEntityTypeConfiguration<Estado>
    {
        public void Configure(EntityTypeBuilder<Estado> builder)
        {
            builder.ToTable("Estados");
            builder.Property(p => p.Clave).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Description).IsRequired().HasMaxLength(200);
        }
    }

    public class MunicipioConfiguration : IEntityTypeConfiguration<Municipio>
    {
        public void Configure(EntityTypeBuilder<Municipio> builder)
        {
            builder.ToTable("Municipios");
            builder.Property(p => p.Clave).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Description).IsRequired().HasMaxLength(200);
            builder
            .HasMany(p => p.CodigosPostales)
            .WithOne(p => p.Municipio)
            .HasForeignKey(p => p.MunicipioId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        }
    }

    public class CodigoPostalConfiguration : IEntityTypeConfiguration<CodigoPostal>
    {
        public void Configure(EntityTypeBuilder<CodigoPostal> builder)
        {
            builder.ToTable("CodigosPostales");
            builder.Property(p => p.Clave).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Description).IsRequired().HasMaxLength(200);
            builder
            .HasMany(p => p.Colonias)
            .WithOne(p => p.CodigoPostal)
            .HasForeignKey(p => p.CodigoPostalId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ColoniaConfiguration : IEntityTypeConfiguration<Colonia>
    {
        public void Configure(EntityTypeBuilder<Colonia> builder)
        {
            builder.ToTable("Colonias");
            builder.Property(p => p.Clave).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Description).IsRequired().HasMaxLength(200);
        }
    }
}
