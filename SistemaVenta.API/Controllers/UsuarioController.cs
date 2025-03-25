using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.API.Utilidad;
using Microsoft.AspNetCore.Authorization;
using SistemaVenta.Utility;

namespace SistemaVenta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly JwtGenerador _jwtGenerador;

        public UsuarioController(IUsuarioService usuarioService, JwtGenerador jwtGenerador)
        {
            _usuarioService = usuarioService;
            _jwtGenerador = jwtGenerador;
        }

        [HttpGet]
        [Route("lista")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Lista()
        {
            // Crear una instancia de la clase para manejar la respuesta de la API.
            var rsp = new Response<List<UsuarioDTO>>();

            try
            {
                // Si la respuesta es true, obtenie la lista de usuarios desde el servicio.
                rsp.status = true;
                rsp.value = await _usuarioService.Lista();
                rsp.msg = "Los datos se han obtenido correctamente";
            }
            catch (Exception ex)
            {
                // Si la respuesta es false, muestra el mensaje de error.
                rsp.status = false;
                rsp.msg = ex.Message;
            }
            return Ok(rsp);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginRequest)
        {
            var rsp = new Response<SesionDTO>();

            try
            {
                var usuarioSesion = await _usuarioService.ValidarCredenciales(loginRequest.Email, loginRequest.Clave);

                if (usuarioSesion == null)
                {
                    rsp.status = false;
                    rsp.msg = "Credenciales incorrectas";
                    return Unauthorized(rsp);
                }

                // Generar el token JWT
                var token = _jwtGenerador.GenerarToken(
                    usuarioSesion.IdUsuario.ToString(),
                    usuarioSesion.Email ?? "",
                    usuarioSesion.RolDescripcion ?? ""
                );

                usuarioSesion.Token = token;

                rsp.status = true;
                rsp.value = usuarioSesion;
                rsp.msg = "Login realizado correctamente";
            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;
            }

            return Ok(rsp);
        }


        [HttpPost]
        [Route("guardar")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Guardar([FromBody] UsuarioDTO usuario)
        {
            var rsp = new Response<UsuarioDTO>();

            try
            {
                rsp.status = true;
                rsp.value = await _usuarioService.Crear(usuario);
                rsp.msg = "El registro fue exitoso";

            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;
            }

            return Ok(rsp);
        }

        [HttpPut]
        [Route("editar")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Editar([FromBody] UsuarioDTO usuario)
        {
            var rsp = new Response<bool>();

            try
            {
                rsp.status = true;
                rsp.value = await _usuarioService.Editar(usuario);
                rsp.msg = "Actualización realizada con éxito";
            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;
            }

            return Ok(rsp);
        }

        [HttpDelete]
        [Route("eliminar/{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var rsp = new Response<bool>();

            try
            {
                rsp.status = true;
                rsp.value = await _usuarioService.Eliminar(id);
                rsp.msg = "Eliminación realizada con éxito";
            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;
            }

            return Ok(rsp);
        }
    }
}
