using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaCidadesRaio.Models
{
    public class BuscaCidadesRaioViewModel
    {
        public string? DddSelecionado { get; set; }
        public int? CidadeOrigemId { get; set; }
        public double? RaioKm { get; set; }
        public int? QuantidadeMaxima { get; set; }

        public CidadeModel? CidadeOrigem { get; set; }

        public List<SelectListItem> Ddds { get; set; } = new();
        public List<SelectListItem> Cidades { get; set; } = new();
        public List<CidadeRaioResultadoModel> Resultados { get; set; } = new();
    }
}