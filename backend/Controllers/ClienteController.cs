using BilleteraCriptoProg3.Data;
using BilleteraCriptoProg3.DTOs;
using BilleteraCriptoProg3.Entities;
using BilleteraCriptoProg3.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BilleteraCriptoProg3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clientes = await _clienteService.GetClientesAsync();
            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var cliente = await _clienteService.GetClienteByIdAsync(id);
            if (cliente == null) return NotFound("Cliente no encontrado.");
            return Ok(cliente);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ClienteDTO dto)
        {
            var clientenew = await _clienteService.CreateClienteAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = clientenew.Id }, clientenew);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ClienteDTO dto)
        {
            var actualizado = await _clienteService.UpdateClienteAsync(id, dto);
            if (actualizado == null) return NotFound("Cliente no encontrado.");
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _clienteService.DeleteClienteAsync(id);
            if (!eliminado) return NotFound("Cliente no encontrado.");
            return NoContent();
        }
    }
}
