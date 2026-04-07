using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaCidadesRaio.Models;
using SistemaCidadesRaio.Services;

namespace SistemaCidadesRaio.Controllers
{
    public class CidadesRaioController : Controller
    {
        private readonly CidadeCsvService _cidadeService;

        public CidadesRaioController(CidadeCsvService cidadeService)
        {
            _cidadeService = cidadeService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var vm = new BuscaCidadesRaioViewModel
            {
                RaioKm = 100,
                QuantidadeMaxima = 10
            };

            CarregarDdds(vm);
            CarregarCidades(vm);

            return View(vm);
        }

        [HttpPost]
        public IActionResult Index(BuscaCidadesRaioViewModel vm)
        {
            CarregarDdds(vm);
            CarregarCidades(vm);

            if (!vm.CidadeOrigemId.HasValue || !vm.RaioKm.HasValue || !vm.QuantidadeMaxima.HasValue)
                return View(vm);

            vm.Resultados = _cidadeService.ObterCidadesPorRaio(
                vm.CidadeOrigemId.Value,
                vm.RaioKm.Value,
                vm.QuantidadeMaxima.Value
            );

            return View(vm);
        }

        private void CarregarDdds(BuscaCidadesRaioViewModel vm)
        {
            vm.Ddds = _cidadeService.ObterDdds()
                .Select(ddd => new SelectListItem
                {
                    Value = ddd,
                    Text = ddd
                })
                .ToList();
        }

        private void CarregarCidades(BuscaCidadesRaioViewModel vm)
        {
            var cidades = string.IsNullOrWhiteSpace(vm.DddSelecionado)
                ? new List<CidadeModel>()
                : _cidadeService.ObterCidadesPorDdd(vm.DddSelecionado);

            vm.Cidades = cidades
                .Select(x => new SelectListItem
                {
                    Value = x.CidadeId.ToString(),
                    Text = $"{x.Nome}/{x.UF} - DDD {x.Ddd}"
                })
                .ToList();
        }
    }
}