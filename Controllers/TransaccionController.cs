using BilleteraCriptoProg3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            => await _context.Transacciones.Where(t => t.idCliente == clienteId).OrderByDescending(t => t.Datetime).ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Transaccion>> Post(TransaccionDTO dto)
        {
            var http = _httpClientFactory.CreateClient();
            string url = $"https://criptoya.com/api/satoshitango/{dto.CodigoCripto}/ars";
            var respuesta = await http.GetFromJsonAsync<CriptoYaRespuesta>(url);
            if (dto.CantCripto <= 0) return BadRequest("La cantidad de cripto debe ser mayor a 0 (cero).");
            if (respuesta == null || respuesta.totalCriptoYa == 0) return BadRequest("No se obtuvo el precio de la criptomoneda.");
            double total = dto.CantCripto * respuesta.totalCriptoYa;
            var transmicion = new Transaccion
            {
                Metodo = dto.Metodo,
                idCliente = dto.ClienteId,
                CodigoCripto = dto.CodigoCripto,
                CantCripto = dto.CantCripto,
                Datetime = dto.Datetime,
                Dinero = Math.Round(total, 2)
            };
            _context.Transacciones.Add(transmicion);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = transmicion.Id }, transmicion);
        }
    }
}
