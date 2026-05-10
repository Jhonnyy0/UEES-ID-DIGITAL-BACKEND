namespace UeesDigital.Domain.Entities
{
    public class Estudiante
    {
        public Guid   Id        { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName  { get; set; } = string.Empty;
        public string FullName  => $"{FirstName} {LastName}";
        public string Email     { get; set; } = string.Empty;
        public int    Carnet    { get; set; }
        public int    IdCarrera { get; set; }

        public Carrera              Carrera  { get; set; } = null!;
        public ICollection<Tramite> Tramites { get; set; } = [];
    }
}
