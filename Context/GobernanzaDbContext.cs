using API_Gobernanza_Digital.Models;
using Microsoft.EntityFrameworkCore;

public class GobernanzaDbContext : DbContext
{
    // Constructor para la Inyección de Dependencias
    public GobernanzaDbContext(DbContextOptions<GobernanzaDbContext> options)
        : base(options)
    {}

    // Definición de los DbSets (las tablas)
    public DbSet<Contribuyente> Contribuyentes { get; set; }
    public DbSet<Servicio> Servicios { get; set; }
    public DbSet<Boleta> Boletas { get; set; }
    public DbSet<PasarelaPago> PasarelasDePago { get; set; }
    public DbSet<ContribuyenteServicio> ContribuyenteServicios { get; set; }

    // Aquí se configura el modelo usando Fluent API
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Configuración de Contribuyente ---
        modelBuilder.Entity<Contribuyente>(entity =>
        {
            entity.ToTable("Contribuyentes"); // Nombre de la tabla
            entity.HasKey(c => c.Id); // Clave Primaria

            entity.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Apellido).HasMaxLength(100);
            entity.Property(c => c.RazonSocial).HasMaxLength(200);
            entity.Property(c => c.Identificacion).IsRequired().HasMaxLength(20);
            entity.Property(c => c.Email).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Domicilio).HasMaxLength(250);

            // Relación 1-a-N con Boletas
            entity.HasMany(c => c.Boletas)
                  .WithOne(b => b.Contribuyente)
                  .HasForeignKey(b => b.ContribuyenteId);
        });

        // --- Configuración de Servicio ---
        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.ToTable("Servicios");
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Nombre).IsRequired().HasMaxLength(150);
            entity.Property(s => s.Descripcion).HasMaxLength(500);
            entity.Property(s => s.MontoBase).IsRequired().HasColumnType("decimal(18, 2)");

            // Relación 1-a-N con Boletas
            entity.HasMany(s => s.Boletas)
                  .WithOne(b => b.Servicio)
                  .HasForeignKey(b => b.ServicioId);
        });

        // --- Configuración de Boleta ---
        modelBuilder.Entity<Boleta>(entity =>
        {
            entity.ToTable("Boletas");
            entity.HasKey(b => b.Id);

            entity.Property(b => b.MontoTotal).IsRequired().HasColumnType("decimal(18, 2)");
            entity.Property(b => b.CodigoPagoElectronico).IsRequired().HasMaxLength(100);
            entity.Property(b => b.FechaPago).IsRequired(false); // Permite nulos
        });

        // --- Configuración de PasarelaPago ---
        modelBuilder.Entity<PasarelaPago>(entity =>
        {
            entity.ToTable("PasarelasDePago");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(p => p.UrlBaseApi).IsRequired().HasMaxLength(255);
            entity.Property(p => p.ApiKey).HasMaxLength(500);
        });

        // --- Configuración de ContribuyenteServicio (El Desafío N-a-N) ---
        modelBuilder.Entity<ContribuyenteServicio>(entity =>
        {
            entity.ToTable("ContribuyenteServicios");
            
            // 1. Definir la Clave Primaria Compuesta
            entity.HasKey(cs => new { cs.ContribuyenteId, cs.ServicioId });

            // 2. Configurar la relación con Contribuyente
            entity.HasOne(cs => cs.Contribuyente)
                  .WithMany(c => c.ContribuyenteServicios)
                  .HasForeignKey(cs => cs.ContribuyenteId);

            // 3. Configurar la relación con Servicio
            entity.HasOne(cs => cs.Servicio)
                  .WithMany(s => s.ContribuyenteServicios)
                  .HasForeignKey(cs => cs.ServicioId);
        });
    }
}