using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using MyApp.Models;
using MyApp.Repositories;
using MyApp.Services;

namespace MyApp.Tests.Services
{
    [TestFixture]
    public class FornecedorServiceTests
    {
        private Mock<IFornecedorRepository> _fornecedorRepositoryMock;
        private IFornecedorService _fornecedorService;

        [SetUp]
        public void Setup()
        {
            _fornecedorRepositoryMock = new Mock<IFornecedorRepository>();
            _fornecedorService = new FornecedorService(_fornecedorRepositoryMock.Object);
        }

        private void SetupAdicionarCallback(Action<Fornecedor> callback)
        {
            _fornecedorRepositoryMock
                .Setup(r => r.AdicionarAsync(It.IsAny<Fornecedor>()))
                .Callback<Fornecedor>(f => callback(f))
                .Returns(Task.CompletedTask);
        }

        [Test]
        public async Task CriarFornecedorAsync_NumericoValido_DeveCriarFornecedor()
        {
            // Arrange
            var request = new FornecedorRequest
            {
                Nome = "Fornecedor Numerico",
                Cnpj = "11444777000161", // CNPJ numérico válido
                TipoPessoa = "EIRELI"
            };

            // Simula que o repositório define o Id
            SetupAdicionarCallback(f => f.Id = 1);

            // Act
            var response = await _fornecedorService.CriarFornecedorAsync(request);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(1, response.Id);
            Assert.AreEqual(request.Nome, response.Nome);
            Assert.AreEqual(request.Cnpj, response.Cnpj); // Should be exactly as informed (in uppercase if applicable)
            Assert.AreEqual(request.TipoPessoa, response.TipoPessoa);
            _fornecedorRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Fornecedor>()), Times.Once);
        }

        [Test]
        public async Task CriarFornecedorAsync_AlfanumericoValido_DeveCriarFornecedor()
        {
            // Arrange
            // Utilizando um CNPJ alfanumérico válido calculado:
            // Escolhemos: "A2345678901242" onde:
            // Para os 12 primeiros dígitos: A -> 17, 2,3,4,5,6,7,8,9,0,1,2
            // Primeiro dígito verificador esperado: 4
            // Segunda etapa com os 13 primeiros dígitos: esperado DV: 2
            var request = new FornecedorRequest
            {
                Nome = "Fornecedor Alfanumerico",
                Cnpj = "A2345678901242",
                TipoPessoa = "EIRELI"
            };
            
            SetupAdicionarCallback(f => f.Id = 2);

            // Act
            var response = await _fornecedorService.CriarFornecedorAsync(request);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(2, response.Id);
            Assert.AreEqual(request.Nome, response.Nome);
            Assert.AreEqual(request.Cnpj.ToUpper(), response.Cnpj);
            Assert.AreEqual(request.TipoPessoa, response.TipoPessoa);
            _fornecedorRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Fornecedor>()), Times.Once);
        }

        [Test]
        public void CriarFornecedorAsync_MEIComAlfanumerico_DeveLancarException()
        {
            // Arrange
            var request = new FornecedorRequest
            {
                Nome = "Fornecedor MEI",
                Cnpj = "A2345678901242", // Alfanumérico
                TipoPessoa = "MEI"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _fornecedorService.CriarFornecedorAsync(request));
            Assert.AreEqual("MEI aceita somente CNPJ numérico", ex.Message);

            // Verifica que o repositório não foi chamado
            _fornecedorRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Fornecedor>()), Times.Never);
        }

        [Test]
        public void CriarFornecedorAsync_CnpjComTamanhoInvalido_DeveLancarException()
        {
            // Arrange
            var request = new FornecedorRequest
            {
                Nome = "Fornecedor Tamanho Invalido",
                Cnpj = "1234567890123", // Apenas 13 caracteres
                TipoPessoa = "EIRELI"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _fornecedorService.CriarFornecedorAsync(request));
            Assert.AreEqual("O CNPJ deve ter exatamente 14 caracteres.", ex.Message);
            _fornecedorRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Fornecedor>()), Times.Never);
        }

        [Test]
        public void CriarFornecedorAsync_CnpjComCaracteresInvalidos_DeveLancarException()
        {
            // Arrange
            var request = new FornecedorRequest
            {
                Nome = "Fornecedor Formato Invalido",
                Cnpj = "A2345@78901242", // Contém caractere inválido '@'
                TipoPessoa = "EIRELI"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _fornecedorService.CriarFornecedorAsync(request));
            Assert.AreEqual("O formato do CNPJ está incorreto. Permite apenas dígitos e letras A-Z.", ex.Message);
            _fornecedorRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Fornecedor>()), Times.Never);
        }

        [Test]
        public void CriarFornecedorAsync_CnpjComDigitoVerificadorIncorreto_DeveLancarException()
        {
            // Arrange
            // Utilizando um CNPJ alfanumérico onde o dígito verificador está errado.
            // Base válido seria "A2345678901242"; modificaremos para "A2345678901243".
            var request = new FornecedorRequest
            {
                Nome = "Fornecedor DV Incorreto",
                Cnpj = "A2345678901243",
                TipoPessoa = "EIRELI"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _fornecedorService.CriarFornecedorAsync(request));
            Assert.AreEqual("O CNPJ não passou na conferência interna do dígito verificador", ex.Message);
            _fornecedorRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Fornecedor>()), Times.Never);
        }

        [Test]
        public async Task AtualizarFornecedorAsync_FornecedorExistente_DeveAtualizarFornecedor()
        {
            // Arrange
            int fornecedorId = 10;
            var fornecedorExistente = new Fornecedor
            {
                Id = fornecedorId,
                Nome = "Fornecedor Antigo",
                Cnpj = "11444777000161",
                TipoPessoa = "EIRELI"
            };

            var request = new FornecedorRequest
            {
                Nome = "Fornecedor Atualizado",
                Cnpj = "A2345678901242",
                TipoPessoa = "EIRELI"
            };

            _fornecedorRepositoryMock.Setup(r => r.ObterPorIdAsync(fornecedorId)).ReturnsAsync(fornecedorExistente);
            _fornecedorRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<Fornecedor>())).Returns(Task.CompletedTask);

            // Act
            var response = await _fornecedorService.AtualizarFornecedorAsync(fornecedorId, request);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(fornecedorId, response.Id);
            Assert.AreEqual(request.Nome, response.Nome);
            Assert.AreEqual(request.Cnpj.ToUpper(), response.Cnpj);
            Assert.AreEqual(request.TipoPessoa, response.TipoPessoa);
            _fornecedorRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Fornecedor>()), Times.Once);
        }

        [Test]
        public async Task ListarFornecedoresAsync_DeveRetornarListaDeFornecedores()
        {
            // Arrange
            var fornecedores = new List<Fornecedor>
            {
                new Fornecedor { Id = 1, Nome = "Fornecedor 1", Cnpj = "11444777000161", TipoPessoa = "EIRELI" },
                new Fornecedor { Id = 2, Nome = "Fornecedor 2", Cnpj = "A2345678901242", TipoPessoa = "EIRELI" }
            };
            
            _fornecedorRepositoryMock.Setup(r => r.ListarAsync()).ReturnsAsync(fornecedores);

            // Act
            var responseList = await _fornecedorService.ListarFornecedoresAsync();

            // Assert
            Assert.AreEqual(2, responseList.Count);
            Assert.IsTrue(responseList.Any(f => f.Id == 1 && f.Cnpj == "11444777000161"));
            Assert.IsTrue(responseList.Any(f => f.Id == 2 && f.Cnpj == "A2345678901242"));
        }

        [Test]
        public async Task ObterFornecedorPorIdAsync_FornecedorExistente_DeveRetornarFornecedor()
        {
            // Arrange
            int fornecedorId = 5;
            var fornecedor = new Fornecedor
            {
                Id = fornecedorId,
                Nome = "Fornecedor Teste",
                Cnpj = "11444777000161",
                TipoPessoa = "EIRELI"
            };
            
            _fornecedorRepositoryMock.Setup(r => r.ObterPorIdAsync(fornecedorId)).ReturnsAsync(fornecedor);

            // Act
            var response = await _fornecedorService.ObterFornecedorPorIdAsync(fornecedorId);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(fornecedorId, response.Id);
        }

        [Test]
        public async Task ObterFornecedorPorIdAsync_FornecedorInexistente_DeveRetornarNull()
        {
            // Arrange
            int fornecedorId = 99;
            _fornecedorRepositoryMock.Setup(r => r.ObterPorIdAsync(fornecedorId)).ReturnsAsync((Fornecedor)null);

            // Act
            var response = await _fornecedorService.ObterFornecedorPorIdAsync(fornecedorId);

            // Assert
            Assert.IsNull(response);
        }
    }
}
