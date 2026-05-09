using System;
using System.Collections.Generic;
using System.Text;

namespace UeesDigital.Domain.Entities
{
    public class Facultad
    {
        public int IdFacultad { get; set; }

        public string Nombre { get; set; }

        public string Codigo { get; set; }

        public ICollection<Carrera> Carreras { get; set; }

    }
}
