using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApp.Models;
using MyApp.Repositories;

namespace MyApp.Services
{
    public class FornecedorService : IFornecedorService
    {
        private readonly IFornecedorRepository _fornecedorRepository;

        public FornecedorService(IFornecedorRepository fornecedorRepository)
        {
            _fornecedorRepository = fornecedorRepository;
        }

        public async Task<FornecedorResponse> CriarFornecedorAsync(FornecedorRequest request)
        {
            ValidarFornecedor(request);

            var fornecedor = new Fornecedor
            {
                Nome = request.Nome,
                Cnpj = request.Cnpj.ToUpper(),
                TipoPessoa = request.TipoPessoa
            };

            await _fornecedorRepository.AdicionarAsync(fornecedor);
            
            return MapearParaResponse(fornecedor);
        }

        public async Task<FornecedorResponse> AtualizarFornecedorAsync(int id, FornecedorRequest request)
        {
            var fornecedorExistente = await _fornecedorRepository.ObterPorIdAsync(id);
            if (fornecedorExistente == null)
                return null;

            ValidarFornecedor(request);

            fornecedorExistente.Nome = request.Nome;
            fornecedorExistente.Cnpj = request.Cnpj.ToUpper();
            fornecedorExistente.TipoPessoa = request.TipoPessoa;
            
            await _fornecedorRepository.AtualizarAsync(fornecedorExistente);

            return MapearParaResponse(fornecedorExistente);
        }

        public async Task<List<FornecedorResponse>> ListarFornecedoresAsync()
        {
            var lista = await _fornecedorRepository.ListarAsync();
            return lista.Select(f => MapearParaResponse(f)).ToList();
        }

        public async Task<FornecedorResponse> ObterFornecedorPorIdAsync(int id)
        {
            var fornecedor = await _fornecedorRepository.ObterPorIdAsync(id);
            if (fornecedor == null)
                return null;
            return MapearParaResponse(fornecedor);
        }

        private void ValidarFornecedor(FornecedorRequest request)
        {
            if (string.IsNullOrEmpty(request.Cnpj) || request.Cnpj.Length != 14)
                throw new Exception("O CNPJ deve ter exatamente 14 caracteres.");

            string cnpj = request.Cnpj.ToUpper();

            // Verifica se possui apenas dígitos ou letras (A-Z) e dígitos
            foreach (char c in cnpj)
            {
                if (!char.IsDigit(c) && !(char.IsLetter(c) && c >= 'A' && c <= 'Z'))
                {
                    throw new Exception("O formato do CNPJ está incorreto. Permite apenas dígitos e letras A-Z.");
                }
            }

            bool cnpjNumerico = cnpj.All(char.IsDigit);
            // Regra para MEI
            if (request.TipoPessoa.ToUpper() == "MEI" && !cnpjNumerico)
                throw new Exception("MEI aceita somente CNPJ numérico");

            // Validação do dígito verificador
            if (!ValidarDigitoVerificador(cnpj))
                throw new Exception("O CNPJ não passou na conferência interna do dígito verificador");
        }

        private bool ValidarDigitoVerificador(string cnpj)
        {
            // Converter cada caractere para um valor numérico: se for dígito, usa o valor; se letra, usa ASCII - 48, conforme especificação.
            int[] valores = new int[14];
            for (int i = 0; i < 14; i++)
            {
                char c = cnpj[i];
                if (char.IsDigit(c))
                    valores[i] = (int)char.GetNumericValue(c);
                else if (char.IsLetter(c))
                    valores[i] = ((int)c) - 48;
                else
                    return false; // Caso inesperado
            }

            // Cálculo do primeiro dígito verificador
            int soma = 0;
            int peso = 2;
            for (int i = 0; i < 12; i++)
            {
                soma += valores[i] * peso;
                peso = (peso == 9 ? 2 : peso + 1);
            }
            int resto = soma % 11;
            int digito1 = (resto < 2) ? 0 : 11 - resto;
            if (digito1 != valores[12])
                return false;

            // Cálculo do segundo dígito verificador
            soma = 0;
            peso = 2;
            for (int i = 0; i < 13; i++)
            {
                soma += valores[i] * peso;
                peso = (peso == 9 ? 2 : peso + 1);
            }
            resto = soma % 11;
            int digito2 = (resto < 2) ? 0 : 11 - resto;
            if (digito2 != valores[13])
                return false;

            return true;
        }

        private FornecedorResponse MapearParaResponse(Fornecedor fornecedor)
        {
            return new FornecedorResponse
            {
                Id = fornecedor.Id,
                Nome = fornecedor.Nome,
                Cnpj = fornecedor.Cnpj,
                TipoPessoa = fornecedor.TipoPessoa
            };
        }
    }
}
