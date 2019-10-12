using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ApiTest.Models
{
    public class Ciudad
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public bool EsCapital { get; set; }
        
        [JsonIgnore]
        [ForeignKey(nameof(IdPais))]
        public Pais Pais { get; set; }
        public long IdPais { get; set; }
    }
}