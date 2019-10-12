using System.Collections.Generic;

namespace ApiTest.Models.Dtos
{
    public class PaisDetailDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Sigla { get; set; }
        public string Capital { get; set; }
        public ICollection<Ciudad> Ciudades { get; set; }
    }
}