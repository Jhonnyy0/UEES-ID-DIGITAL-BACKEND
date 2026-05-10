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

            builder.Entity<Facultad>().HasKey(f => f.IdFacultad);
            builder.Entity<Carrera>().HasKey(c => c.IdCarrera);
            builder.Entity<Estudiante>().HasKey(e => e.Id);
            builder.Entity<FechaDisponible>().HasKey(f => f.IdFechaDisponible);
            builder.Entity<HorarioDisponible>().HasKey(h => h.IdHorario);
            builder.Entity<Tramite>().HasKey(t => t.IdTramite);

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

            builder.Entity<Facultad>().HasData(
                new Facultad { IdFacultad = 1, Nombre = "Facultad de Ingeniería", Codigo = "FI" },
                new Facultad { IdFacultad = 2, Nombre = "Facultad de Ciencias Económicas", Codigo = "FCE" },
                new Facultad { IdFacultad = 3, Nombre = "Facultad de Ciencias Jurídicas", Codigo = "FCJ" },
                new Facultad { IdFacultad = 4, Nombre = "Facultad de Ciencias Sociales", Codigo = "FCS" },
                new Facultad { IdFacultad = 5, Nombre = "Facultad de Ciencias de la Salud", Codigo = "FCSA" }
            );

            builder.Entity<Carrera>().HasData(
                new Carrera { IdCarrera = 1, IdFacultad = 1, Nombre = "Ingeniería en Desarrollo de Software y Ciencia de Datos", Id = new Guid("aaaaaaaa-0001-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) },
                new Carrera { IdCarrera = 2, IdFacultad = 1, Nombre = "Ingeniería en Tecnologías Emergentes y Robótica", Id = new Guid("aaaaaaaa-0002-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) },
                new Carrera { IdCarrera = 3, IdFacultad = 1, Nombre = "Técnico en Ciberseguridad", Id = new Guid("aaaaaaaa-0003-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) },

                new Carrera { IdCarrera = 4, IdFacultad = 2, Nombre = "Licenciatura en Administración de Empresas", Id = new Guid("aaaaaaaa-0004-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) },
                new Carrera { IdCarrera = 5, IdFacultad = 2, Nombre = "Licenciatura en Marketing", Id = new Guid("aaaaaaaa-0005-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) },
                new Carrera { IdCarrera = 6, IdFacultad = 2, Nombre = "Técnico en Marketing Digital", Id = new Guid("aaaaaaaa-0006-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) },
                new Carrera { IdCarrera = 7, IdFacultad = 2, Nombre = "Licenciatura en Relaciones Públicas con especialidad en Marketing", Id = new Guid("aaaaaaaa-0007-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) },

                new Carrera { IdCarrera = 8, IdFacultad = 3, Nombre = "Licenciatura en Ciencias Jurídicas", Id = new Guid("aaaaaaaa-0008-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) },
                new Carrera { IdCarrera = 9, IdFacultad = 3, Nombre = "Licenciatura en Relaciones y Negocios Internacionales", Id = new Guid("aaaaaaaa-0009-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) },

                new Carrera { IdCarrera = 10, IdFacultad = 4, Nombre = "Licenciatura en Psicología", Id = new Guid("aaaaaaaa-0010-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) },
                new Carrera { IdCarrera = 11, IdFacultad = 4, Nombre = "Licenciatura en Lenguas Modernas en Inglés y Francés", Id = new Guid("aaaaaaaa-0011-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) },
                new Carrera { IdCarrera = 12, IdFacultad = 4, Nombre = "Licenciatura en Teología", Id = new Guid("aaaaaaaa-0012-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) },
                new Carrera { IdCarrera = 13, IdFacultad = 4, Nombre = "Licenciatura en Traducción e Interpretación del Idioma Inglés", Id = new Guid("aaaaaaaa-0013-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) },
                new Carrera { IdCarrera = 14, IdFacultad = 4, Nombre = "Profesorado y Licenciatura en Educación Inicial y Parvularia", Id = new Guid("aaaaaaaa-0014-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) },

                new Carrera { IdCarrera = 15, IdFacultad = 5, Nombre = "Doctorado en Medicina", Id = new Guid("aaaaaaaa-0015-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) },
                new Carrera { IdCarrera = 16, IdFacultad = 5, Nombre = "Licenciatura en Enfermería", Id = new Guid("aaaaaaaa-0016-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) },
                new Carrera { IdCarrera = 17, IdFacultad = 5, Nombre = "Licenciatura en Nutrición y Dietética", Id = new Guid("aaaaaaaa-0017-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) },
                new Carrera { IdCarrera = 18, IdFacultad = 5, Nombre = "Técnico en Enfermería", Id = new Guid("aaaaaaaa-0018-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) },
                new Carrera { IdCarrera = 19, IdFacultad = 5, Nombre = "Doctorado en Cirugía Dental", Id = new Guid("aaaaaaaa-0019-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) },
                new Carrera { IdCarrera = 20, IdFacultad = 5, Nombre = "Técnico en Asistencia Odontológica", Id = new Guid("aaaaaaaa-0020-0000-0000-000000000000"), IsDelete = false, CreatedAt = new DateTime(2024, 1, 1) }
            );
        }
    }
}
