using System.ComponentModel.DataAnnotations;

namespace ApiTest.Models.Dtos
{
    public class PaisDto
    {
        [Required]
        public string Nombre { get; set; }
        public string Sigla { get; set; }
    }
}