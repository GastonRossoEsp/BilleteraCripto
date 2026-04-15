using BilleteraCriptoProg3.DTOs;
using BilleteraCriptoProg3.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BilleteraCriptoProg3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransaccionController : ControllerBase
    {
        private readonly ITransaccionService _transaccionService;
        public TransaccionController(ITransaccionService transaccionService)
        {
            _transaccionService = transaccionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var transacciones = await _transaccionService.GetTransaccionesAsync();
            return Ok(transacciones);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var transacciones = await _transaccionService.GetTransaccionByIdAsync(id);
            if (transacciones == null) return NotFound("Transacción no encontrada.");
            return Ok(transacciones);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransaccionRequestDTO dto)
        {
            var transaccionNew = await _transaccionService.CreateTransaccionAsync(dto);
            return Ok(transaccionNew);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateTransaccionDto dto)
        {
            try
            {
                var updated = await _transaccionService.UpdateTransaccionAsync(id, dto);
                if (!updated) return NotFound("Transacción no encontrada.");
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _transaccionService.DeleteTransaccionAsync(id);
            if (!eliminado) return NotFound("Transacción no encontrada.");
            return NoContent();
        }
    }
}
