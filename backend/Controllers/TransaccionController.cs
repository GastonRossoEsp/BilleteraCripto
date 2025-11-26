using Azure;
using BilleteraCriptoProg3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BilleteraCriptoProg3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransaccionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public TransaccionController(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaccion>>> Get()
            => await _context.Transacciones.OrderByDescending(t => t.Datetime).ToListAsync();

        [HttpGet("{clienteId}")]
        public async Task<ActionResult<IEnumerable<Transaccion>>> Get(int clienteId)
            => await _context.Transacciones.Where(t => t.ClienteId == clienteId).OrderByDescending(t => t.Datetime).ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Transaccion>> Post(TransaccionDTO dto)
        {
            try
            {
            if (dto.CantCripto <= 0) return BadRequest("La cantidad de cripto debe ser mayor a 0 (cero).");
            if (dto.Metodo != "purchase" && dto.Metodo != "sale") return BadRequest("Metodo invalido: Debe colocar 'purchase' o 'sale'.");

            var http = _httpClientFactory.CreateClient();
            string url = $"https://criptoya.com/api/satoshitango/{dto.CodigoCripto}/ars";

                var res = await http.GetFromJsonAsync<CriptoYaRespuesta>(url);
                if (res == null || res.totalAsk == 0) return BadRequest("No se obtuvo el precio de la criptomoneda.");
                if (dto.CantCripto <= 0) return BadRequest("La cantidad de criptomonedas debe ser mayor a 0 (cero)");
                var total = dto.CantCripto * res.totalAsk;

                var transaccion = new Transaccion
                {
                    Metodo = dto.Metodo,
                    ClienteId = dto.ClienteId,
                    CodigoCripto = dto.CodigoCripto,
                    CantCripto = dto.CantCripto,
                    Datetime = dto.Datetime,
                    Dinero = Math.Round(total, 2)
                };
                _context.Transacciones.Add(transaccion);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(Get), new { id = transaccion.Id }, transaccion);
            }
            catch (Exception ex) {return BadRequest($"Error al procesar la respuesta de CriptoYa: {ex.Message} - {ex.InnerException?.Message}"); }


        }
    }
}
