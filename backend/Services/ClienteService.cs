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
        public ClienteService(AppDbContext context)
        {
            _context = context;
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
    }
}
