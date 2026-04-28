using BilleteraCriptoProg3.DTOs;
namespace BilleteraCriptoProg3.Services.Interfaces
{
    public interface IClienteService
    {
        Task<List<ClienteDTO>> GetClientesAsync();
        Task<ClienteDTO?> GetClienteByIdAsync(int id);
        Task<ClienteDTO> CreateClienteAsync(ClienteDTO dto);
        Task<ClienteDTO?> UpdateClienteAsync(int id, ClienteDTO dto);
        Task<bool> DeleteClienteAsync(int id);
    }
}
