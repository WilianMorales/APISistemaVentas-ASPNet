using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Model;

namespace SistemaVenta.BLL.Servicios
{
    public class RolService : IRolService
    {
        private readonly IGenericRepository<Rol> _rolRepository;
        private readonly IMapper _mapper;

        public RolService(IGenericRepository<Rol> rolRepository, IMapper mapper)
        {
            _rolRepository = rolRepository;
            _mapper = mapper;
        }

        // Obtiene la lista de roles del sistema en formato DTO.
        public async Task<List<RolDTO>> Lista()
        {
            try
            {
                // Consulta la lista de roles desde el repositorio.
                var listaRoles = await _rolRepository.Consultar();

                // Convertir el modelo rol a RolDTO en forma de lista
                return _mapper.Map<List<RolDTO>>(listaRoles.ToList());
            } catch {
                throw;
            }
        }
    }
}
