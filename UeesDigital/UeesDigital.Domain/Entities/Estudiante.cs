namespace UeesDigital.Domain.Entities{
    public  class Estudiante
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName{
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public int Carnet { get; set; }
        public int IdCarrera { get; set; }

        public Carrera Carrera { get; set; }

        public ICollection<Tramite> Tramites { get; set; }
    }
}
