using System;
using System.Collections.Generic;
using System.Text;

namespace UeesDigital.Domain.Entities
{
    public class HorarioDisponible : BaseEntity
    {
        public int IdHorario { get; set; }
        public int IdFechaDisponible { get; set; }
        public DateTime HoraInicio { get; set; }
        public int CuposMaximos { get; set; }
        public int CuposOcupados { get; set; }
        public bool Activo { get; set; }

        public FechaDisponible FechaDisponible { get; set; }
        public ICollection<Tramite> Tramites { get; set; }
    }
}