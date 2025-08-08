using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using MyApp.Controllers;
using MyApp.Models;
using MyApp.Services;

namespace MyApp.Tests.Controllers
{
    [TestFixture]
    public class FornecedorControllerTests
    {
        private Mock<IFornecedorService> _fornecedorServiceMock;
        private FornecedorController _controller;

        [SetUp]
        public void Setup()
        {
            _fornecedorServiceMock = new Mock<IFornecedorService>();
            _controller = new FornecedorController(_fornecedorServiceMock.Object);
        }

        [Test]
        public async Task CriarFornecedor_FornecedorValido_RetornaCreatedAtAction()
        {
            // Arrange
            var request = new FornecedorRequest { Nome = "Fornecedor Teste", Cnpj = "11444777000161", TipoPessoa = "EIRELI" };
            var response = new FornecedorResponse { Id = 1, Nome = request.Nome, Cnpj = request.Cnpj, TipoPessoa = request.TipoPessoa };
            _fornecedorServiceMock.Setup(s => s.CriarFornecedorAsync(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.CriarFornecedor(request);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(nameof(_controller.ObterFornecedorPorId), createdResult.ActionName);
            Assert.AreEqual(response, createdResult.Value);
        }

        [Test]
        public async Task CriarFornecedor_ServiceLancaException_RetornaBadRequest()
        {
            // Arrange
            var request = new FornecedorRequest { Nome = "Fornecedor Inválido", Cnpj = "A2345678901242", TipoPessoa = "MEI" };
            _fornecedorServiceMock.Setup(s => s.CriarFornecedorAsync(request)).ThrowsAsync(new Exception("MEI aceita somente CNPJ numérico"));

            // Act
            var result = await _controller.CriarFornecedor(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("MEI aceita somente CNPJ numérico", badRequestResult.Value);
        }

        [Test]
        public async Task AtualizarFornecedor_FornecedorExistente_RetornaOk()
        {
            // Arrange
            int fornecedorId = 5;
            var request = new FornecedorRequest { Nome = "Fornecedor Atualizado", Cnpj = "11444777000161", TipoPessoa = "EIRELI" };
            var response = new FornecedorResponse { Id = fornecedorId, Nome = request.Nome, Cnpj = request.Cnpj, TipoPessoa = request.TipoPessoa };
            _fornecedorServiceMock.Setup(s => s.AtualizarFornecedorAsync(fornecedorId, request)).ReturnsAsync(response);

            // Act
            var result = await _controller.AtualizarFornecedor(fornecedorId, request);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(response, okResult.Value);
        }

        [Test]
        public async Task AtualizarFornecedor_FornecedorInexistente_RetornaNotFound()
        {
            // Arrange
            int fornecedorId = 10;
            var request = new FornecedorRequest { Nome = "Fornecedor Atualizado", Cnpj = "11444777000161", TipoPessoa = "EIRELI" };
            _fornecedorServiceMock.Setup(s => s.AtualizarFornecedorAsync(fornecedorId, request)).ReturnsAsync((FornecedorResponse)null);

            // Act
            var result = await _controller.AtualizarFornecedor(fornecedorId, request);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task ListarFornecedores_RetornaOkComLista()
        {
            // Arrange
            var lista = new List<FornecedorResponse>
            {
                new FornecedorResponse { Id = 1, Nome = "Fornecedor 1", Cnpj = "11444777000161", TipoPessoa = "EIRELI" },
                new FornecedorResponse { Id = 2, Nome = "Fornecedor 2", Cnpj = "A2345678901242", TipoPessoa = "EIRELI" }
            };
            _fornecedorServiceMock.Setup(s => s.ListarFornecedoresAsync()).ReturnsAsync(lista);

            // Act
            var result = await _controller.ListarFornecedores();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(lista, okResult.Value);
        }

        [Test]
        public async Task ObterFornecedorPorId_FornecedorExistente_RetornaOk()
        {
            // Arrange
            int fornecedorId = 3;
            var response = new FornecedorResponse { Id = fornecedorId, Nome = "Fornecedor Teste", Cnpj = "11444777000161", TipoPessoa = "EIRELI" };
            _fornecedorServiceMock.Setup(s => s.ObterFornecedorPorIdAsync(fornecedorId)).ReturnsAsync(response);

            // Act
            var result = await _controller.ObterFornecedorPorId(fornecedorId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(response, okResult.Value);
        }

        [Test]
        public async Task ObterFornecedorPorId_FornecedorInexistente_RetornaNotFound()
        {
            // Arrange
            int fornecedorId = 99;
            _fornecedorServiceMock.Setup(s => s.ObterFornecedorPorIdAsync(fornecedorId)).ReturnsAsync((FornecedorResponse)null);

            // Act
            var result = await _controller.ObterFornecedorPorId(fornecedorId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
