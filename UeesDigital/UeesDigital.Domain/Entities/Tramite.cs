using System;
using System.Collections.Generic;
using System.Text;

namespace UeesDigital.Domain.Entities
{
    public class Tramite : BaseEntity
    {
        public Guid IdTramite { get; set; }
        public int IdHorario { get; set; }
        public Guid IdEstudiante { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string CodigoConfirmacion { get; set; }

        public TipoTramite TipoTramite { get; set; }
        public EstadoTramite Estado { get; set; }

        public Estudiante Estudiante { get; set; }
        public HorarioDisponible Horario { get; set; }
    }

    public enum TipoTramite
    {
        PrimeraVez,
        Reposicion,
        Modificacion
    }

    public enum EstadoTramite
    {
        Pendiente,
        Completado,
        Cancelado
    }
}