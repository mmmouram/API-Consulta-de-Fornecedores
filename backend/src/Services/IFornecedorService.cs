using System.Collections.Generic;
using System.Threading.Tasks;
using MyApp.Models;

namespace MyApp.Services
{
    public interface IFornecedorService
    {
        Task<FornecedorResponse> CriarFornecedorAsync(FornecedorRequest request);
        Task<FornecedorResponse> AtualizarFornecedorAsync(int id, FornecedorRequest request);
        Task<List<FornecedorResponse>> ListarFornecedoresAsync();
        Task<FornecedorResponse> ObterFornecedorPorIdAsync(int id);
    }
}
