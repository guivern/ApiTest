using System.Collections.Generic;

namespace ApiTest.Models
{
    public class Pais
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Sigla { get; set; }

        public ICollection<Ciudad> Ciudades { get; set; }
    }
}