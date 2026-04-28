using BilleteraCriptoProg3.DTOs;
namespace BilleteraCriptoProg3.Services.Interfaces
{
    public interface ITransaccionService
    {
        Task<List<TransaccionResponseDTO>> GetTransaccionesAsync();
        Task<TransaccionResponseDTO?> GetTransaccionByIdAsync(int id);
        Task<TransaccionResponseDTO> CreateTransaccionAsync(TransaccionRequestDTO dto);
        Task<bool> DeleteTransaccionAsync(int id);
        Task<bool> UpdateTransaccionAsync(int id, UpdateTransaccionDto dto);
    }
}
