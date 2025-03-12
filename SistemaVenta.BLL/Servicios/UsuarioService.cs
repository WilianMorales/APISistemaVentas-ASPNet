using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemaVenta.BLL.Servicios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _usuarioRepository;
        private readonly IMapper _mapper;

        public UsuarioService(IGenericRepository<Usuario> usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        // Método que devuelve una lista de usuarios en formato DTO.
        public async Task<List<UsuarioDTO>> Lista()
        {
            try
            {
                // Obtenciión de datos del Repositorio incluyendo información relacionada con el rol de cada usuario.
                var queryUsuario = await _usuarioRepository.Consultar();
                var listaUsuarios = queryUsuario.Include(rol => rol.IdRolNavigation).ToList();

                // Se mapea la lista de usuarios a una lista de 'UsuarioDTO' y la devuelve.
                return _mapper.Map<List<UsuarioDTO>>(listaUsuarios);
            }
            catch
            {
                throw;
            }
        }

        // Método para validar las credenciales de un usuario mediante su email y clave.
        public async Task<SesionDTO> ValidarCredenciales(string email, string clave)
        {
            try
            {
                var queryUsuario = await _usuarioRepository.Consultar(u => 
                   u.Email == email &&
                   u.Clave == clave
                );

                // Validar si no existe el usuario o es null y devolver un mensaje.
                if (queryUsuario.FirstOrDefault() == null)
                    throw new TaskCanceledException("El usuario no existe.");

                // Obtiene el primer usuario encontrado, incluyendo información relacionada con el rol.
                Usuario devolverUsuario = queryUsuario.Include(rol => rol.IdRolNavigation).First();

                // Mapea el usuario a un objeto DTO de sesión y lo devuelve.
                return _mapper.Map<SesionDTO>(devolverUsuario);
            }
            catch
            {
                throw;
            }
        }

        // Método para crear un nuevo usuario apartir de un modelo usuarioDTO proporcionado.
        public async Task<UsuarioDTO> Crear(UsuarioDTO modelo)
        {
            try
            {
                // Convierte el modelo recibido de tipo DTO a un objeto Usuario utilizando AutoMapper y lo crea en el repositorio.
                var usuarioCreado = await _usuarioRepository.Crear(_mapper.Map<Usuario>(modelo));

                if (usuarioCreado.IdUsuario == 0)
                    throw new TaskCanceledException("No se pudo crear el Usuario.");

                // Consulta el usuario recién creado por su ID, para obtener más detalles, incluyendo sus relaciones.
                var query = await _usuarioRepository.Consultar(u => u.IdUsuario == usuarioCreado.IdUsuario);

                usuarioCreado = query.Include(rol => rol.IdRolNavigation).First();

                // Mapea el usuario a un objeto de tipo DTO, para devolverlo como resultado del método.
                return _mapper.Map<UsuarioDTO>(usuarioCreado);
            }
            catch
            {
                throw;
            }
        }

        // Método para editar los datos de un usuario apartir de un modelo usuarioDTO proporcionado.
        public async Task<bool> Editar(UsuarioDTO modelo)
        {
            try
            {
                var usuarioModelo = _mapper.Map<Usuario>(modelo);

                // Obtiene el usuario existente del repositorio según su IdUsuario.
                var usuarioEncontrado = await _usuarioRepository.Obtener(u => u.IdUsuario == usuarioModelo.IdUsuario);

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El usuario no existe.");

                // Actualiza los datos del usuario existente con la datos del UsuarioDTO proporcionado.
                usuarioEncontrado.Nombre = usuarioModelo.Nombre;
                usuarioEncontrado.Email = usuarioModelo.Email;
                usuarioEncontrado.IdRol = usuarioModelo.IdRol;
                usuarioEncontrado.Clave = usuarioModelo.Clave;
                usuarioEncontrado.EsActivo = usuarioModelo.EsActivo;

                // Intenta editar el usuario en el repositorio y guarda la respuesta.
                bool respuesta = await _usuarioRepository.Editar(usuarioEncontrado);

                // Si la edición no fue exitosa, se lanza una excepción.
                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar el usuario.");

                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        // Método para eliminar un usuario según su ID.
        public async Task<bool> Eliminar(int id)
        {
            try
            {
                // Busca el usuario en el repositorio según su IdUsuario.
                var usuarioEncontrado = await _usuarioRepository.Obtener(u => u.IdUsuario == id);

                // Valida si encontró o no un usuario
                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El usuario no existe.");

                // Intenta eliminar el usuario del repositorio y guarda la respuesta.
                bool respuesta = await _usuarioRepository.Eliminar(usuarioEncontrado);

                // Si la eliminación no fue exitosa, se lanza una excepción.
                if (!respuesta)
                    throw new TaskCanceledException("No se pudo elimininar al usuario.");

                return respuesta;
            }
            catch
            {
                throw;
            }
        }

    }
}
