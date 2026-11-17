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
            var codigoCripto = transaccionDto.CodigoCripto.Trim().ToLower();
            var metodo = transaccionDto.Metodo.Trim().ToLower();

            var client = _httpFactory.CreateClient();
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var criptoData = await client.GetFromJsonAsync<CriptoYaRespuesta>($"https://criptoya.com/api/{codigoCripto}/ars/1", options);
            
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
                CodigoCripto = codigoCripto,
                Metodo = metodo,
                ClienteId = transaccionDto.ClienteId,
                CantCripto = transaccionDto.CantCripto,
                Datetime = transaccionDto.Datetime,
                Dinero = Math.Round((decimal)(transaccionDto.CantCripto * precio), 2)
            };

            if (metodo == "purchase" || metodo == "buy" || metodo == "compra")
            {
                // Validar que el cliente tenga suficiente saldo para comprar.
                if (entity.Dinero > cliente.Saldo)
                {
                    throw new ArgumentException(
                        $"No se puede realizar la compra. " +
                        $"El cliente dispone de {cliente.Saldo:f2} " +
                        $"y necesita {entity.Dinero:f2}."
                    );
                }
                cliente.Saldo -= entity.Dinero;
            }
            else if (metodo == "sale" || metodo == "sell" || metodo == "venta")
            {
                var cantidadDisponible = await _context.Transacciones
                .Where(t =>
                    t.ClienteId == transaccionDto.ClienteId &&
                    t.CodigoCripto.ToLower() == transaccionDto.CodigoCripto.ToLower())
                .SumAsync(t => 
                    t.Metodo.ToLower() == "purchase" ||
                    t.Metodo.ToLower() == "buy" ||
                    t.Metodo.ToLower() == "compra"
                        ? t.CantCripto
                        : -t.CantCripto
                );
                if (transaccionDto.CantCripto > cantidadDisponible)
                {
                    throw new ArgumentException(
                        $"No se puede vender {transaccionDto.CantCripto} {transaccionDto.CodigoCripto}. " +
                        $"El cliente dispone solamente de {cantidadDisponible}."
                    );
                }
                cliente.Saldo += entity.Dinero;
            }
            else throw new Exception($"Método no reconocido: {transaccionDto.Metodo}");

            _context.Clientes.Update(cliente);
            _context.Transacciones.Add(entity);
            await _context.SaveChangesAsync();

        var saved = await _context.Transacciones.Include(t => t.Cliente).FirstAsync(t => t.Id == entity.Id);
            return saved.ToResponseDTO();
        }

        public async Task<bool> UpdateTransaccionAsync(int id, UpdateTransaccionDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.CantCripto <= 0) throw new ArgumentException("CantCripto debe ser mayor que 0.");
            if (string.IsNullOrWhiteSpace(dto.CodigoCripto)) throw new ArgumentException("Debe indicar una criptomoneda.");
            var metodoNuevo = NormalizarMetodo(dto.Metodo);
            var transaccion = await _context.Transacciones.FirstOrDefaultAsync(t => t.Id == id);
            if (transaccion == null) throw new KeyNotFoundException("La transaccion no existe");
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == transaccion.ClienteId);
            if (cliente == null) throw new KeyNotFoundException("El cliente de la transaccion no existe.");
            var metodoAnterior = NormalizarMetodo(transaccion.Metodo);
            decimal dineroAnterior = transaccion.Dinero;
            //estima el precio unitario implicito de la operacion original
            decimal precioUnitario = dineroAnterior / (decimal)transaccion.CantCripto;

            //calcula el efecto monetario correspondiente a la nueva cantidad
            //esto no modifica el importe historico de la transaccion
            decimal dineroNuevoParaSaldo = Math.Round(precioUnitario * (decimal)dto.CantCripto, 2, MidpointRounding.AwayFromZero);

            // Revierte el efecto de la operacion original
            AjustarSaldo(cliente, metodoAnterior, dineroAnterior, revertir: true);

            // Actualiza los datos editables
            transaccion.CodigoCripto = dto.CodigoCripto.Trim().ToLowerInvariant();
            transaccion.Metodo = metodoNuevo;
            transaccion.CantCripto = dto.CantCripto;
            transaccion.Datetime = dto.Datetime;
            // IMPORTANTE: no modifico transaccion.Dinero ,,,, la edicion conserva el importe original.

            // aplcia el efecto de la operacion editada
            AjustarSaldo(cliente, metodoNuevo, dineroNuevoParaSaldo, revertir: false);

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteTransaccionAsync(int id)
        {
            var transaccion = await _context.Transacciones.FirstOrDefaultAsync(t => t.Id == id);
            if (transaccion == null) return false;
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == transaccion.ClienteId);
            if (cliente == null) throw new KeyNotFoundException("El cliente de la transaccion no existe.");
            var metodo = NormalizarMetodo(transaccion.Metodo);
            AjustarSaldo(cliente, metodo, transaccion.Dinero, revertir: true);
            _context.Transacciones.Remove(transaccion);
            await _context.SaveChangesAsync();
            return true;
        }

        private static string NormalizarMetodo(string? metodo)
        {
            return metodo?.Trim().ToLowerInvariant() switch
            {
                "purchase" or "compra" => "purchase",
                "sale" or "venta" => "sale",
                _ => throw new ArgumentException("El metodo debe ser compra/purchase o venta/sale")
            };
        }

        private static void AjustarSaldo(Cliente cliente, string metodo, decimal dinero, bool revertir)
        {
            if (metodo == "purchase")
            {
                cliente.Saldo += revertir ? dinero : -dinero;
            } else
            {
                cliente.Saldo += revertir ? -dinero : dinero; // la normalizacion garantiza que sea una venta
            }
        }
    }
}
