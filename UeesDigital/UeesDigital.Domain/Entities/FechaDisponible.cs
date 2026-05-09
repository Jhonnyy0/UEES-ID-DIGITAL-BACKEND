using System;
using System.Collections.Generic;
using System.Text;

namespace UeesDigital.Domain.Entities
{
    public class FechaDisponible : BaseEntity
    {
        public int IdFechaDisponible { get; set; }
        public DateTime Fecha { get; set; }
        public bool Activo { get; set; }

        public ICollection<HorarioDisponible> Horarios { get; set; }    
    }
}
