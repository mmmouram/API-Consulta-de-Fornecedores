using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Models;
using MyApp.Data;

namespace MyApp.Repositories
{
    public class FornecedorRepository : IFornecedorRepository
    {
        private readonly FornecedorDbContext _context;

        public FornecedorRepository(FornecedorDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Fornecedor fornecedor)
        {
            _context.Fornecedores.Add(fornecedor);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Fornecedor fornecedor)
        {
            _context.Fornecedores.Update(fornecedor);
            await _context.SaveChangesAsync();
        }

        public async Task<Fornecedor> ObterPorIdAsync(int id)
        {
            return await _context.Fornecedores.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<List<Fornecedor>> ListarAsync()
        {
            return await _context.Fornecedores.ToListAsync();
        }
    }
}
