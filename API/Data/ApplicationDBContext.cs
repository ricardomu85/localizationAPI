using Microsoft.EntityFrameworkCore;
using System.Reflection;
using API.Models.Localization;

namespace API.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public virtual DbSet<Pais>? Paises { get; set; }
        public virtual DbSet<Estado>? Estados { get; set; }
        public virtual DbSet<Municipio>? Municipios { get; set; }
        public virtual DbSet<CodigoPostal>? CodigosPostales { get; set; }
        public virtual DbSet<Colonia>? Colonias { get; set; }
    }
}
