using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UeesDigital.Domain.Entities;
using UeesDigital.Infrastructure.Identity;

namespace UeesDigital.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<AppIdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Facultad> Facultades { get; set; }
        public DbSet<Carrera> Carreras { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<FechaDisponible> FechasDisponibles { get; set; }
        public DbSet<HorarioDisponible> HorariosDisponibles { get; set; }
        public DbSet<Tramite> Tramites { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Facultad>()
                .HasIndex(f => f.Codigo).IsUnique();

            builder.Entity<Estudiante>()
                .Property(e => e.Id).HasDefaultValueSql("NEWID()");

            builder.Entity<Tramite>()
                .Property(t => t.IdTramite).HasDefaultValueSql("NEWID()");

            builder.Entity<Tramite>()
                .HasIndex(t => t.CodigoConfirmacion).IsUnique();

            builder.Entity<Tramite>()
                .Property(t => t.Estado)
                .HasConversion<string>()
                .HasDefaultValue(EstadoTramite.Pendiente);

            builder.Entity<Tramite>()
                .Property(t => t.TipoTramite)
                .HasConversion<string>();

            builder.Entity<Tramite>()
                .Property(t => t.FechaRegistro).HasDefaultValueSql("GETDATE()");

            builder.Entity<HorarioDisponible>()
                .Property(h => h.CuposMaximos).HasDefaultValue(5);

            builder.Entity<Carrera>()
                .HasOne(c => c.Facultad)
                .WithMany(f => f.Carreras)
                .HasForeignKey(c => c.IdFacultad);

            builder.Entity<Estudiante>()
                .HasOne(e => e.Carrera)
                .WithMany(c => c.Estudiantes)
                .HasForeignKey(e => e.IdCarrera);

            builder.Entity<HorarioDisponible>()
                .HasOne(h => h.FechaDisponible)
                .WithMany(f => f.Horarios)
                .HasForeignKey(h => h.IdFechaDisponible);

            builder.Entity<Tramite>()
                .HasOne(t => t.Estudiante)
                .WithMany(e => e.Tramites)
                .HasForeignKey(t => t.IdEstudiante);

            builder.Entity<Tramite>()
                .HasOne(t => t.Horario)
                .WithMany(h => h.Tramites)
                .HasForeignKey(t => t.IdHorario);
        }
    }
}