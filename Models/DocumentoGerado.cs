namespace SistemaCidadesRaio.Models
{
    public class DocumentoGerado
    {
        public int DocumentoGeradoId { get; set; }
        public string NomeArquivo { get; set; } = string.Empty;
        public DateTime DataGeracao { get; set; } = DateTime.UtcNow;
        public int QuantidadeCidades { get; set; }
    }
}