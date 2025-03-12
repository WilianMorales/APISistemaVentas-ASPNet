using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.API.Utilidad;
using SistemaVenta.BLL.Servicios;

namespace SistemaVenta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        [Route("lista")]
        public async Task<IActionResult> Lista()
        {
            // Crear una instancia de la clase para manejar la respuesta de la API.
            var rsp = new Response<List<CategoriaDTO>>();

            try
            {
                // Si la respuesta es true, obtenie la lista de categorias desde el servicio.
                rsp.status = true;
                rsp.value = await _categoriaService.Lista();
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
    }
}
