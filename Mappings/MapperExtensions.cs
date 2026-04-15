using BilleteraCriptoProg3.DTOs;
using BilleteraCriptoProg3.Entities;

namespace BilleteraCriptoProg3.Mappings
{
    public static class MapperExtensions // esto es para no repetir codigo en los servicios y controladores
    {
        public static TransaccionResponseDTO ToResponseDTO(this Transaccion t)
        {
            return new TransaccionResponseDTO
            {
                Id = t.Id,
                CodigoCripto = t.CodigoCripto,
                Metodo = t.Metodo,
                CantCripto = t.CantCripto,
                Dinero = t.Dinero,
                Datetime = t.Datetime,
                // esta linea hace una validacion para ver si el nombre es nulo, el ? es la consulta y lo que esta entre : es como responderia el sistema.
                ClienteNombre = t.Cliente != null ? t.Cliente.Nombre : ""
            };
        }

        public static ClienteDTO ToDTO(this Cliente c)
        {
            return new ClienteDTO
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Email = c.Email,
                Saldo = c.Saldo
            };
        }

        public static Cliente ToEntity(this ClienteDTO dto)
        {
            return new Cliente
            {
                Id = dto.Id,
                Nombre = dto.Nombre ?? string.Empty,
                Email = dto.Email ?? string.Empty
            };
        }

        public static Transaccion ToEntity(this TransaccionRequestDTO dto, double precioCripto)
        {
            return new Transaccion
            {
                CodigoCripto = dto.CodigoCripto ?? string.Empty,
                Metodo = dto.Metodo ?? string.Empty,
                ClienteId = dto.ClienteId,
                CantCripto = dto.CantCripto,
                Dinero = 0,
                Datetime = dto.Datetime == default ? DateTime.UtcNow : dto.Datetime
            };
        }
    }
}
