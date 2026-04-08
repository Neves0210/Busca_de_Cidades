namespace SistemaCidadesRaio.Models
{
    public class CidadeEnviada
    {
        public int CidadeEnviadaId { get; set; }
        public int CidadeId { get; set; }
        public string NomeDocumento { get; set; } = string.Empty;
        public DateTime DataEnvio { get; set; } = DateTime.UtcNow;
    }
}