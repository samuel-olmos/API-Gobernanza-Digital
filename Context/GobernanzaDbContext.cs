using API_Gobernanza_Digital.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Gobernanza_Digital.Context
{
    public class GobernanzaDbContext : DbContext
    {
        // Constructor (Sin cambios)
        public GobernanzaDbContext(DbContextOptions<GobernanzaDbContext> options)
            : base(options)
        { }

        // --- Definición de los DbSets (ACTUALIZADO) ---
        // Se agregan las nuevas tablas
        public DbSet<Contribuyente> Contribuyentes { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Boleta> Boletas { get; set; }
        public DbSet<PasarelaPago> PasarelasDePago { get; set; }
        public DbSet<ContribuyenteServicio> ContribuyenteServicios { get; set; }
        
        public DbSet<TipoContribuyente> TiposContribuyente { get; set; } // <-- NUEVO
        public DbSet<Estado> Estados { get; set; } // <-- NUEVO
        public DbSet<Frecuencia> Frecuencias { get; set; } // <-- NUEVO
        public DbSet<Periodo> Periodos { get; set; } // <-- NUEVO

        // Aquí se configura el modelo usando Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configuración de Contribuyente (ACTUALIZADO) ---
            modelBuilder.Entity<Contribuyente>(entity =>
            {
                entity.ToTable("Contribuyentes");
                entity.HasKey(c => c.Id);

                // Propiedades (Sin cambios, ya estaban bien)
                entity.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Apellido).HasMaxLength(100);
                entity.Property(c => c.RazonSocial).HasMaxLength(200);
                entity.Property(c => c.Identificacion).IsRequired().HasMaxLength(20);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Domicilio).HasMaxLength(250);

                // --- Relación (ACTUALIZADA) ---
                // Se quita la relación directa con Boleta
                // Se agrega la relación con TipoContribuyente
                entity.HasOne(c => c.Tipo)
                      .WithMany(t => t.Contribuyentes)
                      .HasForeignKey(c => c.TipoId); // <-- NUEVO
            });

            // --- Configuración de Servicio (ACTUALIZADO) ---
            modelBuilder.Entity<Servicio>(entity =>
            {
                entity.ToTable("Servicios");
                entity.HasKey(s => s.Id);

                // Propiedades (Sin cambios, ya estaban bien)
                entity.Property(s => s.Nombre).IsRequired().HasMaxLength(150);
                entity.Property(s => s.Descripcion).HasMaxLength(500);
                entity.Property(s => s.MontoBase).IsRequired().HasColumnType("decimal(18, 2)");

                // --- Relación (ACTUALIZADA) ---
                // Se quita la relación directa con Boleta
                // Se agrega la relación con Frecuencia
                entity.HasOne(s => s.Frecuencia)
                      .WithMany(f => f.Servicios)
                      .HasForeignKey(s => s.FrecuenciaId); // <-- NUEVO
            });
            
            // --- Configuración de Boleta (ACTUALIZADO) ---
            modelBuilder.Entity<Boleta>(entity =>
            {
                entity.ToTable("Boletas");
                entity.HasKey(b => b.Id);

                // Propiedades (Sin cambios, ya estaban bien)
                entity.Property(b => b.MontoTotal).IsRequired().HasColumnType("decimal(18, 2)");
                entity.Property(b => b.CodigoPagoElectronico).IsRequired().HasMaxLength(100);
                entity.Property(b => b.FechaPago).IsRequired(false);

                // --- Relaciones (TODAS NUEVAS, según el diagrama) ---
                
                // Relación 1-a-N con ContribuyenteServicio
                entity.HasOne(b => b.ContribuyenteServicio)
                      .WithMany(cs => cs.Boletas)
                      .HasForeignKey(b => b.ContribuyenteServicioId);

                // Relación 1-a-N con Periodo
                entity.HasOne(b => b.Periodo)
                      .WithMany(p => p.Boletas)
                      .HasForeignKey(b => b.PeriodoId);

                // Relación 1-a-N con Estado
                entity.HasOne(b => b.Estado)
                      .WithMany(e => e.Boletas)
                      .HasForeignKey(b => b.EstadoId);
            });

            // --- Configuración de ContribuyenteServicio (ACTUALIZADO) ---
            modelBuilder.Entity<ContribuyenteServicio>(entity =>
            {
                entity.ToTable("ContribuyenteServicios");

                // 1. Definir la Clave Primaria (Simple, no compuesta)
                entity.HasKey(cs => cs.Id); // <-- ACTUALIZADO (PK simple)

                // 2. Definir un índice único para evitar duplicados
                entity.HasIndex(cs => new { cs.ContribuyenteId, cs.ServicioId }).IsUnique(); // <-- NUEVO (Buena práctica)

                // 3. Configurar las propiedades de fecha
                entity.Property(cs => cs.FechaInicio).HasColumnType("date"); // <-- NUEVO
                entity.Property(cs => cs.FechaFin).HasColumnType("date").IsRequired(false); // <-- NUEVO (Permite nulos)

                // 4. Configurar la relación con Contribuyente
                entity.HasOne(cs => cs.Contribuyente)
                      .WithMany(c => c.ContribuyenteServicios)
                      .HasForeignKey(cs => cs.ContribuyenteId);

                // 5. Configurar la relación con Servicio
                entity.HasOne(cs => cs.Servicio)
                      .WithMany(s => s.ContribuyenteServicios)
                      .HasForeignKey(cs => cs.ServicioId);
            });

            // --- Configuración de PasarelaPago (Sin cambios) ---
            modelBuilder.Entity<PasarelaPago>(entity =>
            {
                entity.ToTable("PasarelasDePago");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(p => p.UrlBaseApi).IsRequired().HasMaxLength(255);
                entity.Property(p => p.ApiKey).HasMaxLength(500); // Permite nulos por defecto
            });

            // --- Configuración de Tablas Catálogo (NUEVAS) ---
            
            modelBuilder.Entity<TipoContribuyente>(entity =>
            {
                entity.ToTable("TiposContribuyente");
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Nombre).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<Estado>(entity =>
            {
                entity.ToTable("Estados");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<Frecuencia>(entity =>
            {
                entity.ToTable("Frecuencias");
                entity.HasKey(f => f.Id);
                entity.Property(f => f.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(f => f.MesesIntervalo).IsRequired();
            });

            modelBuilder.Entity<Periodo>(entity =>
            {
                entity.ToTable("Periodos");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.PeriodoFiscal).IsRequired().HasMaxLength(7); // "2025/01"
                entity.Property(p => p.FechaVencimiento).IsRequired().HasColumnType("date");
                
                // Índice único para evitar períodos duplicados
                entity.HasIndex(p => p.PeriodoFiscal).IsUnique(); 
            });
        }
    }
}