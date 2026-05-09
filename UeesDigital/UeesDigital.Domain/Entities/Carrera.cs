using System;
using System.Collections.Generic;
using System.Text;

namespace UeesDigital.Domain.Entities
{
    public class Carrera
    {
        public int IdCarrera { get; set; }
        public int IdFacultad { get; set; }
        public string Nombre { get; set; }

        public Facultad Facultad { get; set; }
        public ICollection<Estudiante> Estudiantes { get; set; } 
    }
}
