namespace ApiTest.Models.Dtos
{
    public class CiudadListDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string PaisNombre { get; set; }
        public long IdPais { get; set; }
        public bool EsCapital { get; set; }
    }
}