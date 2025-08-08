using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using MyApp.Models;
using MyApp.Services;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FornecedorController : ControllerBase
    {
        private readonly IFornecedorService _fornecedorService;

        public FornecedorController(IFornecedorService fornecedorService)
        {
            _fornecedorService = fornecedorService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarFornecedor([FromBody] FornecedorRequest request)
        {
            try
            {
                var fornecedor = await _fornecedorService.CriarFornecedorAsync(request);
                return CreatedAtAction(nameof(ObterFornecedorPorId), new { id = fornecedor.Id }, fornecedor);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarFornecedor(int id, [FromBody] FornecedorRequest request)
        {
            try
            {
                var fornecedor = await _fornecedorService.AtualizarFornecedorAsync(id, request);
                if (fornecedor == null)
                    return NotFound();
                return Ok(fornecedor);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListarFornecedores()
        {
            var fornecedores = await _fornecedorService.ListarFornecedoresAsync();
            return Ok(fornecedores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterFornecedorPorId(int id)
        {
            var fornecedor = await _fornecedorService.ObterFornecedorPorIdAsync(id);
            if (fornecedor == null)
                return NotFound();
            return Ok(fornecedor);
        }
    }
}
