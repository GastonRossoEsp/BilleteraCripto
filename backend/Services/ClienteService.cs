using BilleteraCriptoProg3.Data;
using BilleteraCriptoProg3.DTOs;
using BilleteraCriptoProg3.Mappings;
using BilleteraCriptoProg3.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BilleteraCriptoProg3.Services
{
    public class ClienteService : IClienteService
    {
        private readonly AppDbContext _context;
        private readonly CriptoYaService _criptoYaService;
        public ClienteService(AppDbContext context, CriptoYaService criptoYaService)
        {
            _context = context;
            _criptoYaService = criptoYaService;
        }

        public async Task<List<ClienteDTO>> GetClientesAsync()
        {
            var clientes = await _context.Clientes.ToListAsync();
            return clientes.Select(c => c.ToDTO()).ToList();
        }

        public async Task<ClienteDTO?> GetClienteByIdAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            return cliente?.ToDTO();
        }

        public async Task<ClienteDTO> CreateClienteAsync(ClienteDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var cliente = dto.ToEntity();

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return cliente.ToDTO();
        }

        public async Task<ClienteDTO?> UpdateClienteAsync(int id, ClienteDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            Console.WriteLine($"Saldo recibido: {dto.Saldo}");

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return null;

            cliente.Nombre = dto.Nombre;
            cliente.Email = dto.Email;
            cliente.Saldo = dto.Saldo;

            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
            return cliente.ToDTO();
        }

        public async Task<bool> DeleteClienteAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return false;
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<EstadoCarteraDTO?> GetEstadoCarteraAsync(int id)
        {
            var cliente = await _context.Clientes.Include(c => c.Transacciones).FirstOrDefaultAsync(c => c.Id == id);
            if (cliente == null) return null;

            var saldos = cliente.Transacciones.GroupBy(t => t.CodigoCripto.Trim().ToLower())
                .Select(grupo => new
                {
                    CodigoCripto = grupo.Key,
                    Cantidad = grupo.Sum(t =>
                    t.Metodo.Trim().ToLower() == "purchase" ||
                    t.Metodo.Trim().ToLower() == "buy" ||
                    t.Metodo.Trim().ToLower() == "compra"
                    ? t.CantCripto
                    : -t.CantCripto
                    )
                }).Where(x => x.Cantidad > 0).ToList();

            var resultado = new EstadoCarteraDTO
            {
                ClienteId = cliente.Id,
                ClienteNombre = cliente.Nombre
            };

            foreach (var saldo in saldos)
            {
                var precioActual = await _criptoYaService.GetPrecioActualAsync(saldo.CodigoCripto);
                var dinero = Math.Round((decimal)(saldo.Cantidad * precioActual), 2);
                resultado.Criptomonedas.Add(new CarteraCriptoDTO
                {
                    CodigoCripto = saldo.CodigoCripto,
                    Cantidad = saldo.Cantidad,
                    Dinero = dinero
                });
                resultado.Total += dinero;
            }
            return resultado;
        }
    }
}
