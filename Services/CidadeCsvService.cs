using SistemaCidadesRaio.Models;
using System.Globalization;

namespace SistemaCidadesRaio.Services
{
    public class CidadeCsvService
    {
        private readonly IWebHostEnvironment _environment;
        private List<CidadeModel>? _cacheCidades;

        public CidadeCsvService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public List<CidadeModel> ObterCidades()
        {
            if (_cacheCidades != null && _cacheCidades.Any())
                return _cacheCidades;

            var caminho = Path.Combine(_environment.ContentRootPath, "Data", "cidades.csv");

            if (!File.Exists(caminho))
                return new List<CidadeModel>();

            var linhas = File.ReadAllLines(caminho);

            if (linhas.Length <= 1)
                return new List<CidadeModel>();

            var separador = DetectarSeparador(linhas[0]);
            var cabecalho = linhas[0].Split(separador).Select(LimparTexto).ToList();

            int idxCodigoIbge = cabecalho.FindIndex(x => x.Equals("CodigoIbge", StringComparison.OrdinalIgnoreCase) || x.Equals("codigo_ibge", StringComparison.OrdinalIgnoreCase));
            int idxNome = cabecalho.FindIndex(x => x.Equals("Nome", StringComparison.OrdinalIgnoreCase) || x.Equals("nome", StringComparison.OrdinalIgnoreCase) || x.Equals("municipio", StringComparison.OrdinalIgnoreCase));
            int idxLatitude = cabecalho.FindIndex(x => x.Equals("Latitude", StringComparison.OrdinalIgnoreCase) || x.Equals("latitude", StringComparison.OrdinalIgnoreCase));
            int idxLongitude = cabecalho.FindIndex(x => x.Equals("Longitude", StringComparison.OrdinalIgnoreCase) || x.Equals("longitude", StringComparison.OrdinalIgnoreCase));
            int idxDdd = cabecalho.FindIndex(x => x.Equals("DDD", StringComparison.OrdinalIgnoreCase) || x.Equals("ddd", StringComparison.OrdinalIgnoreCase));
            int idxCodigoUf = cabecalho.FindIndex(x => x.Equals("codigo_uf", StringComparison.OrdinalIgnoreCase) || x.Equals("CodigoUf", StringComparison.OrdinalIgnoreCase));

            if (idxCodigoIbge < 0 || idxNome < 0 || idxLatitude < 0 || idxLongitude < 0 || idxDdd < 0 || idxCodigoUf < 0)
                return new List<CidadeModel>();

            var lista = new List<CidadeModel>();

            for (int i = 1; i < linhas.Length; i++)
            {
                var linha = linhas[i];

                if (string.IsNullOrWhiteSpace(linha))
                    continue;

                var colunas = linha.Split(separador);

                if (colunas.Length <= Math.Max(idxCodigoUf, Math.Max(idxDdd, Math.Max(idxLongitude, Math.Max(idxLatitude, Math.Max(idxNome, idxCodigoIbge))))))
                    continue;

                var codigoIbgeTexto = LimparTexto(colunas[idxCodigoIbge]);
                var nome = LimparTexto(colunas[idxNome]);
                var latitudeTexto = LimparTexto(colunas[idxLatitude]);
                var longitudeTexto = LimparTexto(colunas[idxLongitude]);
                var dddTexto = LimparTexto(colunas[idxDdd]);
                var codigoUfTexto = LimparTexto(colunas[idxCodigoUf]);

                if (!long.TryParse(codigoIbgeTexto, out var codigoIbge))
                    continue;

                if (!int.TryParse(codigoUfTexto, out var codigoUf))
                    continue;

                if (!TryParseDouble(latitudeTexto, out var latitude))
                    continue;

                if (!TryParseDouble(longitudeTexto, out var longitude))
                    continue;

                lista.Add(new CidadeModel
                {
                    CidadeId = (int)codigoIbge,
                    CodigoIbge = codigoIbge,
                    Nome = nome,
                    UF = ObterSiglaUf(codigoUf),
                    Latitude = latitude,
                    Longitude = longitude,
                    Ddd = NormalizarDdd(dddTexto)
                });
            }

            _cacheCidades = lista
                .OrderBy(x => x.Nome)
                .ThenBy(x => x.UF)
                .ToList();

            return _cacheCidades;
        }

        public List<string> ObterDdds()
        {
            return ObterCidades()
                .Where(x => !string.IsNullOrWhiteSpace(x.Ddd))
                .Select(x => x.Ddd)
                .Distinct()
                .OrderBy(x => int.TryParse(x, out var n) ? n : int.MaxValue)
                .ToList();
        }

        public List<CidadeModel> ObterCidadesPorDdd(string ddd)
        {
            var dddNormalizado = NormalizarDdd(ddd);

            return ObterCidades()
                .Where(x => x.Ddd == dddNormalizado)
                .OrderBy(x => x.Nome)
                .ThenBy(x => x.UF)
                .ToList();
        }

        public List<CidadeRaioResultadoModel> ObterCidadesPorRaio(int cidadeOrigemId, double raioKm, int quantidadeMaxima)
        {
            var cidades = ObterCidades();
            var cidadeOrigem = cidades.FirstOrDefault(x => x.CidadeId == cidadeOrigemId);

            if (cidadeOrigem == null)
                return new List<CidadeRaioResultadoModel>();

            return cidades
                .Where(x => x.CidadeId != cidadeOrigemId)
                .Select(x => new CidadeRaioResultadoModel
                {
                    CidadeId = x.CidadeId,
                    CodigoIbge = x.CodigoIbge,
                    Nome = x.Nome,
                    UF = x.UF,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    DistanciaKm = Math.Round(
                        (decimal)GeoHelper.CalcularDistanciaKm(
                            cidadeOrigem.Latitude,
                            cidadeOrigem.Longitude,
                            x.Latitude,
                            x.Longitude), 2)
                })
                .Where(x => x.DistanciaKm <= (decimal)raioKm)
                .OrderBy(x => x.DistanciaKm)
                .Take(quantidadeMaxima)
                .ToList();
        }

        private static char DetectarSeparador(string cabecalho)
        {
            return cabecalho.Contains(';') ? ';' : ',';
        }

        private static string LimparTexto(string valor)
        {
            return valor.Trim().Trim('"');
        }

        private static bool TryParseDouble(string valor, out double resultado)
        {
            valor = LimparTexto(valor);

            if (double.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out resultado))
                return true;

            if (double.TryParse(valor, NumberStyles.Any, new CultureInfo("pt-BR"), out resultado))
                return true;

            valor = valor.Replace(",", ".");

            return double.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out resultado);
        }

        private static string NormalizarDdd(string valor)
        {
            valor = LimparTexto(valor);

            if (string.IsNullOrWhiteSpace(valor))
                return string.Empty;

            var apenasDigitos = new string(valor.Where(char.IsDigit).ToArray());

            if (apenasDigitos.Length == 0)
                return string.Empty;

            if (apenasDigitos.Length == 2)
                return apenasDigitos;

            if (apenasDigitos.Length > 2)
                return apenasDigitos.Substring(apenasDigitos.Length - 2, 2);

            return apenasDigitos;
        }

        private static string ObterSiglaUf(int codigoUf)
        {
            return codigoUf switch
            {
                11 => "RO",
                12 => "AC",
                13 => "AM",
                14 => "RR",
                15 => "PA",
                16 => "AP",
                17 => "TO",
                21 => "MA",
                22 => "PI",
                23 => "CE",
                24 => "RN",
                25 => "PB",
                26 => "PE",
                27 => "AL",
                28 => "SE",
                29 => "BA",
                31 => "MG",
                32 => "ES",
                33 => "RJ",
                35 => "SP",
                41 => "PR",
                42 => "SC",
                43 => "RS",
                50 => "MS",
                51 => "MT",
                52 => "GO",
                53 => "DF",
                _ => string.Empty
            };
        }
    }
}