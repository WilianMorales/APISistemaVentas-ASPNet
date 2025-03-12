using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.API.Utilidad;

namespace SistemaVenta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet]
        [Route("lista")]
        public async Task<IActionResult> Lista()
        {
            var rsp = new Response<List<ProductoDTO>>();

            try
            {
                rsp.status = true;
                rsp.value = await _productoService.Lista();
                rsp.msg = "Los datos se han obtenido correctamente";
            } 
            catch(Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;
            }

            return Ok(rsp);
        }

        [HttpPost]
        [Route("guardar")]
        public async Task<IActionResult> Guardar([FromBody] ProductoDTO producto)
        {
            var rsp = new Response<ProductoDTO>();

            try
            {
                rsp.status = true;
                rsp.value = await _productoService.Crear(producto);
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
        public async Task<IActionResult> Editar([FromBody] ProductoDTO producto)
        {
            var rsp = new Response<bool>();

            try
            {
                rsp.status = true;
                rsp.value = await _productoService.Editar(producto);
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
        public async Task<IActionResult> Eliminar(int id)
        {
            var rsp = new Response<bool>();

            try
            {
                rsp.status = true;
                rsp.value = await _productoService.Eliminar(id);
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
