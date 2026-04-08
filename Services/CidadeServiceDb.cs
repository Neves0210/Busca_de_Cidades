using SistemaCidadesRaio.Data;
using SistemaCidadesRaio.Models;

namespace SistemaCidadesRaio.Services
{
    public class CidadeServiceDb
    {
        private readonly AppDbContext _context;

        public CidadeServiceDb(AppDbContext context)
        {
            _context = context;
        }

        public List<Cidade> ObterCidadesNaoEnviadas(List<CidadeRaioResultadoModel> resultados)
        {
            var cidadesIds = resultados.Select(x => x.CidadeId).ToList();

            var cidadesJaEnviadas = _context.CidadesEnviadas
                .Where(x => cidadesIds.Contains(x.CidadeId))
                .Select(x => x.CidadeId)
                .ToList();

            return resultados
                .Where(x => !cidadesJaEnviadas.Contains(x.CidadeId))
                .Select(x => new Cidade
                {
                    CidadeId = x.CidadeId,
                    CodigoIbge = x.CodigoIbge,
                    Nome = x.Nome,
                    UF = x.UF,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude
                })
                .ToList();
        }
    }
}