using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
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
    public class DashboardService : IDashboardService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IGenericRepository<Producto> _productoRepository;
        private readonly IMapper _mapper;

        public DashboardService(IVentaRepository ventaRepository, IGenericRepository<Producto> produtoRepository, IMapper mapper)
        {
            _ventaRepository = ventaRepository;
            _productoRepository = produtoRepository;
            _mapper = mapper;
        }

        // Método privado para obtener las ventas en un rango de fechas
        private IQueryable<Venta> retornarVentas(IQueryable<Venta> tablaVenta, int restarCantidadDias)
        {
            // Obtener la última fecha de la tabla y restar la cantidad de días
            DateTime? ultimaFecha = tablaVenta.OrderByDescending(v => v.FechaRegistro).Select(v => v.FechaRegistro).First();
            ultimaFecha = ultimaFecha.Value.AddDays(restarCantidadDias);

            // Filtramos las ventas por fechas mayores o iguales a la última fecha calculada
            return tablaVenta.Where(v => v.FechaRegistro.Value.Date >= ultimaFecha.Value.Date);
        }

        // Método privado para obtener el total de ventas de la última semana
        private async Task<int> TotalVentasUltimaSemana()
        {
            int total = 0;

            // Consultar las ventas de la base de datos
            IQueryable<Venta> _ventaQuery = await _ventaRepository.Consultar();

            if (_ventaQuery.Count() > 0)
            {
                // Obtener las ventas de la última semana y contarlas
                var tablaVenta = retornarVentas(_ventaQuery, -7);
                total = tablaVenta.Count();
            }
            return total;
        }

        private async Task<string> TotalIngresosUltimaSemana()
        {
            decimal resultado = 0;

            IQueryable<Venta> _ventaQuery = await _ventaRepository.Consultar();

            if(_ventaQuery.Count() > 0)
            {
                var tablaVenta = retornarVentas(_ventaQuery, -7);

                resultado = tablaVenta.Select(v => v.Total).Sum(v => v.Value);
            }
            return Convert.ToString(resultado, new CultureInfo("es-PE"));
        }

        private async Task<int> TotalProductos()
        {
            // Consultar los productos de la base de datos
            IQueryable<Producto> _productoQuery = await _productoRepository.Consultar();

            // Contar los productos
            int total = _productoQuery.Count();
            return total;
        }

        // Método privado para obtener un diccionario con el total de ventas por fecha de la última semana
        private async Task<Dictionary<string, int>> VentasUltimaSemana()
        {
            Dictionary<string, int> resultado = new Dictionary<string, int>();

            IQueryable<Venta> _ventaQuery = await _ventaRepository.Consultar();

            if (_ventaQuery.Count() > 0)
            {
                var tablaVenta = retornarVentas(_ventaQuery, -7);

                // Agrupar las ventas por fecha y contar la cantidad de ventas para cada fecha
                resultado = tablaVenta
                    .GroupBy(v => v.FechaRegistro.Value.Date).OrderBy(g => g.Key)
                    .Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() })
                    .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total);
            }

            return resultado;
        }

        public async Task<DashboradDTO> Resumen()
        {
            DashboradDTO vmDashboard = new DashboradDTO();

            try
            {
                // Obtenemos el total de ventas, ingresos y productos de la última semana
                vmDashboard.TotalVentas = await TotalVentasUltimaSemana();
                vmDashboard.TotalIngresos = await TotalIngresosUltimaSemana();
                vmDashboard.TotalProductos = await TotalProductos();

                // Inicializamos una lista para almacenar el detalle de las ventas
                List<VentaSemanaDTO> listaVentaSemana = new List<VentaSemanaDTO>();

                // Recorremos el diccionario de ventas y agregamos cada elemento a la lista
                foreach (KeyValuePair<string, int> item in await VentasUltimaSemana())
                {
                    listaVentaSemana.Add(new VentaSemanaDTO()
                    {
                        Fecha = item.Key,
                        Total = item.Value
                    });
                }

                vmDashboard.VentasUltimaSemana = listaVentaSemana;
            }
            catch
            {
                throw;
            }

            return vmDashboard;
        }
    }
}
