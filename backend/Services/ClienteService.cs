using BilleteraCriptoProg3.Data;
using BilleteraCriptoProg3.DTOs;
using BilleteraCriptoProg3.Mappings;
using BilleteraCriptoProg3.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

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

            ValidarDatosCliente(dto);
            var email = dto.Email.Trim().ToLowerInvariant();
            var emailExiste = await _context.Clientes.AnyAsync(c => c.Email.ToLower() == email);
            if (emailExiste) throw new ArgumentException("Ya existe un cliente registrado con ese email.");

            var cliente = dto.ToEntity();
            cliente.Nombre = dto.Nombre.Trim();
            cliente.Email = email;
            //el saldo inicial no se establece desde el formulario de alta, se utiliza la funcion de carga de saldo.
            cliente.Saldo = 0;
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return cliente.ToDTO();
        }

        public async Task<ClienteDTO?> UpdateClienteAsync(int id, ClienteDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            ValidarDatosCliente(dto);

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return null;
            var email = dto.Email.Trim().ToLowerInvariant();
            var emailExiste = await _context.Clientes.AnyAsync(c => c.Id != id && c.Email.ToLower() == email);
            if (emailExiste) throw new ArgumentException("Ya existe otro cliente registrado con ese email.");

            cliente.Nombre = dto.Nombre.Trim();
            cliente.Email = email;

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

            var saldos = cliente.Transacciones.GroupBy(t => t.CodigoCripto.Trim().ToLowerInvariant())
                .Select(grupo => new
                {
                    CodigoCripto = grupo.Key,
                    Cantidad = grupo.Sum(t =>
                    t.Metodo.Trim().ToLowerInvariant() == "purchase" ||
                    t.Metodo.Trim().ToLowerInvariant() == "buy" ||
                    t.Metodo.Trim().ToLowerInvariant() == "compra"
                    ? t.CantCripto
                    : -t.CantCripto
                    )
                }).Where(x => x.Cantidad > 0).ToList();

            var resultado = new EstadoCarteraDTO
            {
                ClienteId = cliente.Id,
                ClienteNombre = cliente.Nombre,
                SaldoDisponible = cliente.Saldo
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
            resultado.PatrimonioTotal = resultado.Total + resultado.SaldoDisponible;
            return resultado;
        }

        public async Task<ClienteDTO> CargarSaldoAsync(int id, decimal monto)
        {
            if (monto <= 0) throw new ArgumentException("El monto a cargar debe ser mayor que cero.");

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) throw new KeyNotFoundException("El cliente no existe");
            cliente.Saldo += monto;
            await _context.SaveChangesAsync();
            return cliente.ToDTO();
        }

        private static void ValidarDatosCliente(ClienteDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException("El nombre del cliente es obligatorio");
            if (string.IsNullOrWhiteSpace(dto.Email)) throw new ArgumentException("El email del cliente es obligatorio");

            try
            {
                var emailIngresado = dto.Email.Trim();
                var email = new MailAddress(emailIngresado);
                if (email.Address != emailIngresado) throw new ArgumentException("El email ingresado no es valido.");
            }
            catch (FormatException)
            {
                throw new ArgumentException("El email ingresado no es valido.");
            }
        }
    }
}
