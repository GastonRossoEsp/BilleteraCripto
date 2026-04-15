using BilleteraCriptoProg3.Data;
using BilleteraCriptoProg3.DTOs;
using BilleteraCriptoProg3.Entities;
using BilleteraCriptoProg3.Mappings;
using BilleteraCriptoProg3.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using BilleteraCriptoProg3.Models.CriptoYa;


namespace BilleteraCriptoProg3.Services
{
    public class TransaccionService : ITransaccionService
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpFactory;
        private readonly ILogger<TransaccionService> _logger;
        public TransaccionService(AppDbContext context, IHttpClientFactory httpFactory, ILogger<TransaccionService> logger)
        {
            _context = context;
            _httpFactory = httpFactory;
            _logger = logger;
        }

        public async Task<List<TransaccionResponseDTO>> GetTransaccionesAsync()
        {
            var transacciones = await _context.Transacciones
                .Include(t => t.Cliente)
                .ToListAsync();
            return transacciones.Select(t => t.ToResponseDTO()).ToList();
        }

        public async Task<TransaccionResponseDTO?> GetTransaccionByIdAsync(int id)
        {
            var transaccion = await _context.Transacciones
                .Include(t => t.Cliente)
                .FirstOrDefaultAsync(t => t.Id == id);
            return transaccion?.ToResponseDTO();
        }

        public async Task<TransaccionResponseDTO> CreateTransaccionAsync(TransaccionRequestDTO transaccionDto)
        {
            if (transaccionDto is null) throw new ArgumentNullException(nameof(transaccionDto));
            if (transaccionDto.CantCripto <= 0) throw new ArgumentException("CantCripto debe ser mayor que 0.", nameof(transaccionDto.CantCripto));

            var cliente = await _context.Clientes.FindAsync(transaccionDto.ClienteId);
            if (cliente == null) throw new Exception("Cliente no encontrado.");

            var client = _httpFactory.CreateClient();
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var criptoData = await client.GetFromJsonAsync<CriptoYaRespuesta>($"https://criptoya.com/api/{transaccionDto.CodigoCripto}/ars/1", options);
            
            // tomamos solo los ask validos (>0)
            var validAsks = criptoData.Values
                .Where(x => x.ask > 0)
                .Select(x => x.ask)
                .ToList();

            if (!validAsks.Any()) throw new Exception("No se pudo obtener un precio válido para la criptomoneda.");

            // Precio promedio realista entre los distintos exchanges, para evitar outliers.
            double precio = validAsks.Average();

            // Crear entidad y calcular Dinero en una línea
            var entity = new Transaccion
            {
                CodigoCripto = transaccionDto.CodigoCripto,
                Metodo = transaccionDto.Metodo,
                ClienteId = transaccionDto.ClienteId,
                CantCripto = transaccionDto.CantCripto,
                Datetime = transaccionDto.Datetime,
                Dinero = Math.Round((decimal)(transaccionDto.CantCripto * precio), 2)
            };
            var metodo = transaccionDto.Metodo.Trim().ToLower();
            if (metodo == "purchase" || metodo == "buy" || metodo == "compra") cliente.Saldo -= (decimal)entity.Dinero;

            else if (metodo == "sale" || metodo == "sell" || metodo == "venta") cliente.Saldo += (decimal)entity.Dinero;

            else throw new Exception($"Método no reconocido: {transaccionDto.Metodo}");

            _context.Clientes.Update(cliente);
            _context.Transacciones.Add(entity);
            await _context.SaveChangesAsync();

        var saved = await _context.Transacciones.Include(t => t.Cliente).FirstAsync(t => t.Id == entity.Id);
            return saved.ToResponseDTO();
        }

        public async Task<bool> UpdateTransaccionAsync(int id, UpdateTransaccionDto dto)
        {
            var transaccion = await _context.Transacciones.FirstOrDefaultAsync(t => t.Id == id);
            if (transaccion == null) throw new KeyNotFoundException("La transaccion no existe");
            if (dto.CantCripto <=0) throw new ArgumentException("CantCripto debe ser mayor que 0.");

            var client = _httpFactory.CreateClient();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            
            var markets = await client.GetFromJsonAsync<CriptoYaRespuesta>($"https://criptoya.com/api/{dto.CodigoCripto}/ars/1", options);
            var market = markets?.Values.FirstOrDefault() ?? throw new Exception("No se encontraron datos del mercado para este cripto.");

            double precio =
                market.ask > 0 ? market.ask :
                market.totalAsk > 0 ? market.totalAsk :
                market.bid > 0 ? market.bid :
                market.totalBid > 0 ? market.totalBid :
                throw new Exception("No se pudo obtener un precio válido para la criptomoneda.");

            transaccion.CodigoCripto = dto.CodigoCripto;
            transaccion.Metodo = dto.Metodo;
            transaccion.CantCripto = dto.CantCripto;
            transaccion.Datetime = dto.Datetime;
            transaccion.Dinero = (decimal)Math.Round(precio * dto.CantCripto, 2);

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteTransaccionAsync(int id)
        {
            var transaccion = await _context.Transacciones.FindAsync(id);
            if (transaccion == null) return false;
            _context.Transacciones.Remove(transaccion);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
