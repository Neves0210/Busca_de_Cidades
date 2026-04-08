using Microsoft.EntityFrameworkCore;
using SistemaCidadesRaio.Models;

namespace SistemaCidadesRaio.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Cidade> Cidades { get; set; }
        public DbSet<CidadeEnviada> CidadesEnviadas { get; set; }
        public DbSet<DocumentoGerado> DocumentosGerados { get; set; }
    }
}