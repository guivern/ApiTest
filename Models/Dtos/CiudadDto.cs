using System.ComponentModel.DataAnnotations;

namespace ApiTest.Models.Dtos
{
    public class CiudadDto
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public long? IdPais { get; set; }
        public bool EsCapital { get; set; }
    }
}