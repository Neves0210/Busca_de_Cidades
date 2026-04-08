namespace SistemaCidadesRaio.Models
{
    public class Cidade
    {
        public int CidadeId { get; set; }
        public long CodigoIbge { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string UF { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Ddd { get; set; } = string.Empty;
    }
}