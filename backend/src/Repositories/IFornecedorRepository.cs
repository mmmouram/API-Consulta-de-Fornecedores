using System.Collections.Generic;
using System.Threading.Tasks;
using MyApp.Models;

namespace MyApp.Repositories
{
    public interface IFornecedorRepository
    {
        Task AdicionarAsync(Fornecedor fornecedor);
        Task AtualizarAsync(Fornecedor fornecedor);
        Task<Fornecedor> ObterPorIdAsync(int id);
        Task<List<Fornecedor>> ListarAsync();
    }
}
